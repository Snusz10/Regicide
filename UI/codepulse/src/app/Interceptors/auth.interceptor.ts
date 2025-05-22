import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';


/** 
 * Add the 'Authorization' cookie to each http action method
 * */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
    const cookieService = inject(CookieService);
    const userToken = cookieService.get('Authorization');
    if (userToken){
        // if we want to add an authorization token to a http action method (contains the `?addAuth=true` string)
        if (request.urlWithParams.indexOf('AddAuth=true') > -1){
            const modifiedRequest = request.clone({
            headers: request.headers.set('Authorization', `Bearer ${userToken}`),
        });
        return next(modifiedRequest);
        }
    }
    return next(request);  
};

