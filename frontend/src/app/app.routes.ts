import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./features/feed/home-redirect.component').then(m => m.HomeRedirectComponent)
      },
      {
        path: 'feed',
        loadComponent: () => import('./features/feed/feed.component').then(m => m.FeedComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/profile/profile-redirect/profile-redirect.component').then(m => m.ProfileRedirectComponent)
      },
      {
        path: 'profile/:id',
        loadComponent: () => import('./features/profile/profile-page/profile-page.component').then(m => m.ProfilePageComponent)
      },
      {
        path: 'notifications',
        loadComponent: () => import('./features/notifications/notifications.component').then(m => m.NotificationsComponent)
      },
      {
        path: 'admin',
        canActivate: [adminGuard],
        loadComponent: () => import('./features/admin/dashboard/dashboard.component').then(m => m.DashboardComponent)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
