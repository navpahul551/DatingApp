import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: Array<User> | null = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getUsers();
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  getUsers() {
    this.http.get<Array<User>>("https://localhost:7111/api/users").subscribe({
      next: response => this.users = response,
      error: err => console.log(err),
      complete: () => console.log("The request is complete!")
    });
  }

  cancelRegisterationMode(event: boolean) {
    this.registerMode = event;
  }

}
