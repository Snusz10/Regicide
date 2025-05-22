import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../Services/auth.service';
import { User } from '../../../Models/Login/user.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
    
    user?: User;

    constructor(private authService: AuthService,
                private router: Router
    ){}
    
    logoutButtonPressed(){
        this.authService.logout();
        this.router.navigateByUrl('/');
    }

    ngOnInit(): void {
        // get the user from the cookies, or not at all
        this.user = this.authService.getUser();
        
        this.authService.subscribeToUser().subscribe({
            next: (response) => {
                this.user = response;
            },
            error: (response) => {
                console.log("Error");
            }
        });    
    }

}
