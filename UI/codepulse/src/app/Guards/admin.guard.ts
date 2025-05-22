import { inject } from '@angular/core';
import { CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../Services/auth.service';
import { jwtDecode } from 'jwt-decode'

/**
 * If the user tries to access any of the /admin pages through punching in the url manually,
 * they will be shown an `unauthorized` message, or will be redirected back to the login page
 * @param route 
 * @param state 
 * @returns true if the user is allowed to access any of the admin pages, false otherwise
 */
export const adminGuard: CanActivateFn = (route, state) => {
    const cookieService = inject(CookieService);
    const authService = inject(AuthService);
    const router = inject(Router);

    const user = authService.getUser();
    var token = cookieService.get('Authorization');

    // if the jwt token has not been given to the user, or we don't have a user
    if (!token || !user){
        return notAllowed(authService, router, state);
    }

    // get the time that the token will expire
    token = token.replace('Bearer ', '');
    const decodedToken: any = jwtDecode(token);
    const expirationDate = decodedToken.exp * 1000;
    const currentTime = new Date().getTime();

    // if the token has expired
    if (expirationDate >= currentTime){
        return notAllowed(authService, router, state);
    }

    // if the user is not an admin
    if (!user.roles.includes('Writer')){
        alert('Unauthorized');
        return false;
    }

    return true;
};

/**
 * If the user is not allowed to navigate to the page, log them out, and put them back
 * to the login page.
 * @param authService 
 * @param router 
 * @param state 
 * @returns 
 */
function notAllowed(authService: AuthService, router: Router, state: RouterStateSnapshot) {
    authService.logout();
    return router.createUrlTree(['/login'], {queryParams : {returnUrl: state.url}});
}

