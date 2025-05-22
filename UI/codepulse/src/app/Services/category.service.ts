import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Category } from '../Models/Category/category.model';
import { environment } from '../../environments/environment';
import { AddCategoryModel } from '../Models/Category/add-category-request.model';
import { UpdateCategoryModel } from '../Models/Category/update-category-request.model';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(private http: HttpClient,
              private cookieService : CookieService
  ) { }

  addCategory(model: AddCategoryModel) : Observable<void>{
    return this.http.post<void>(`${environment.apiBaseUrl}/api/Categories?addAuth=true`, model);
  }

  getAllCategories(): Observable<Category[]>{
    return this.http.get<Category[]>(`${environment.apiBaseUrl}/api/Categories`);
  }

  getCategoryByID(id: string): Observable<Category> {
    return this.http.get<Category>(`${environment.apiBaseUrl}/api/Categories/${id}`)
  }

  updateCategory(id: string, updateCategoryRequest: UpdateCategoryModel): Observable<Category>{
    return this.http.put<Category>(`${environment.apiBaseUrl}/api/Categories/${id}?addAuth=true`, updateCategoryRequest);
  }

  deleteCategory(id: string): Observable<Category>{
    return this.http.delete<Category>(`${environment.apiBaseUrl}/api/Categories/${id}?addAuth=true`);
  }

}
