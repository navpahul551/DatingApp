using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        
        public MessagesController(IUserRepository userRepository, 
            IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var senderUsername = User.GetUsername();

            if(senderUsername == createMessageDto.RecipientUsername)
                return BadRequest("You cannot send messages to yourself!");

            var sender = await _userRepository.GetUserByUsernameAsync(senderUsername);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if(recipient == null) return NotFound("Recipient not found!");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = senderUsername,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message!");
        } 

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUserName = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUserName, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _messageRepository.GetMessage(id);

            if(message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if(message.SenderUsername == username) message.SenderDeleted = true;
            if(message.RecipientUsername == username) message.RecipientDeleted = true;

            if(message.RecipientDeleted && message.SenderDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            if(await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}