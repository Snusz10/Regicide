import { Component } from '@angular/core';
import { LoginRequest } from '../../../../Models/Login/login-request.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../Services/auth.service';
import {CookieService} from 'ngx-cookie-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
    loginRequest: LoginRequest;

    constructor(private authService: AuthService,
                private cookieService: CookieService,
                private router: Router
    ){
        this.loginRequest = {
            email: '',
            password: ''
        };
    }

    onFormSubmit(): void{
        this.authService.login(this.loginRequest).subscribe({
            next: (response) => {
                this.cookieService.set(
                    'Authorization', // type of token
                    `Bearer ${response.token}`,  // the token in question?
                    undefined, // the date the token should expire
                    '/', // the path of the token?
                    undefined, // the domain?
                    true, // the security of the token?
                    'Strict' // ?
                );

                this.authService.setUser({
                    email: response.email,
                    roles: response.roles
                });

                this.router.navigateByUrl('/');
            }
        })
    }
}
