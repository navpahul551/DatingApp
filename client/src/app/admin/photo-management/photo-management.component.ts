import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] | undefined;

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.loadPhotosForModeration();
  }

  loadPhotosForModeration() {
    this.adminService.getPhotosForModeration().subscribe({
      next: photos => {
        this.photos = photos;
      }
    })
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => this.loadPhotosForModeration()
    });
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => this.loadPhotosForModeration()
    })
  }

}
