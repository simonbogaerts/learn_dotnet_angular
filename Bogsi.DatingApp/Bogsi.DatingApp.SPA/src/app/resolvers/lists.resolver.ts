import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../models/user.model';
import { UserService } from '../services/user.service';
import { AlertService } from '../services/alert.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ListsResolver implements Resolve<User[]> {
  pageNumber: number = 1;
  pageSize: number = 5;
  likes: string = 'Likers';

  constructor(
    private userService: UserService,
    private alertService: AlertService,
    private router: Router
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    return this.userService
      .getUsers(this.pageNumber, this.pageSize, null, this.likes)
      .pipe(
        catchError(error => {
          this.alertService.error('Problem retrieving data');
          this.router.navigate(['/home']);

          return of(null);
        })
      );
  }
}
