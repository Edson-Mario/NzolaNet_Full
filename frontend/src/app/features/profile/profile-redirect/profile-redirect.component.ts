import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-profile-redirect',
  standalone: true,
  template: ''
})
export class ProfileRedirectComponent implements OnInit {
  private router = inject(Router);
  private auth = inject(AuthService);

  ngOnInit(): void {
    const id = this.auth.currentUser()?.id;
    if (id) {
      this.router.navigate(['/profile', id]);
    } else {
      this.router.navigate(['/login']);
    }
  }
}
