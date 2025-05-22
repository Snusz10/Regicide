import { Injectable } from '@angular/core';
import { LoginResponse } from '../Models/Login/login-response-model';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginRequest } from '../Models/Login/login-request.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { User } from '../Models/Login/user.model';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    private lszEmail = 'user-email';
    private lszRoles = 'user-roles';

    oUser = new BehaviorSubject<User | undefined>(undefined);

    constructor(private http: HttpClient,
                private cookieService: CookieService
    ) { }

    login(request: LoginRequest): Observable<LoginResponse>{
        return this.http.post<LoginResponse>(`${environment.apiBaseUrl}/api/authentication/login`, {
            email: request.email,
            password: request.password,
        });
    }

    getUser(): User | undefined {
        const email = localStorage.getItem(this.lszEmail);
        const roles = localStorage.getItem(this.lszRoles);

        if (!email || !roles){
            return undefined;
        }
        const user: User = {
            email:  email,
            roles: roles.split(',')
        }
        return user;
    }

    setUser(user: User): void{
        // send an action to the observable saying that we updated the user
        this.oUser.next(user);

        localStorage.setItem(this.lszEmail, user.email);
        localStorage.setItem(this.lszRoles, user.roles.join(','));
        
    }

    subscribeToUser(): Observable<User | undefined> {
        return this.oUser.asObservable();
    }

    logout(){
        localStorage.removeItem(this.lszEmail);
        localStorage.removeItem(this.lszRoles);
        this.cookieService.delete('Authorization', '/');
        this.oUser.next(undefined);
    }

}
