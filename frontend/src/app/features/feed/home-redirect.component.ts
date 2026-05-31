import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-home-redirect',
  standalone: true,
  template: ''
})
export class HomeRedirectComponent implements OnInit {
  private router = inject(Router);
  private auth = inject(AuthService);

  ngOnInit(): void {
    if (this.auth.isAdmin()) {
      this.router.navigate(['/admin']);
    } else {
      this.router.navigate(['/feed']);
    }
  }
}
