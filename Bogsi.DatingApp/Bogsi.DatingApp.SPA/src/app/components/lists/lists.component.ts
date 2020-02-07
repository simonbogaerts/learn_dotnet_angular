import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user.model';
import { Pagination } from 'src/app/models/pagination.model';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';
import { AlertService } from 'src/app/services/alert.service';
import { ActivatedRoute } from '@angular/router';
import { PaginatedResult } from 'src/app/models/PaginatedResult.model';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam: string;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertService: AlertService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe(
      data => {
        this.users = data['data'].result;
        this.pagination = data['data'].pagination;
      },
      error => {
        this.alertService.error(error);
      }
    );

    this.likesParam = 'Likers';
  }

  loadUsers() {
    this.userService
      .getUsers(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        null,
        this.likesParam
      )
      .subscribe(
        (response: PaginatedResult<User[]>) => {
          this.users = response.result;
          this.pagination = response.pagination;
        },
        error => {
          this.alertService.error(error);
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
}
