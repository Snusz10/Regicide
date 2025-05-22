import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BlogPost } from '../Models/BlogPost/blog-post.model';
import { AddBlogPostModel } from '../Models/BlogPost/add-blog-post.model';
import { UpdateBlogPostModel } from '../Models/BlogPost/update-blog-post.model';

@Injectable({
  providedIn: 'root'
})
export class BlogPostService {

  constructor(private http: HttpClient) { }

  addBlogPost(model: AddBlogPostModel) : Observable<void>{
    return this.http.post<void>(`${environment.apiBaseUrl}/api/BlogPost?addAuth=true`, model);
  }

  getAllBlogPosts(): Observable<BlogPost[]>{
    return this.http.get<BlogPost[]>(`${environment.apiBaseUrl}/api/BlogPost`);
  }

  getBlogPostByID(id: string): Observable<BlogPost> {
    return this.http.get<BlogPost>(`${environment.apiBaseUrl}/api/BlogPost/${id}`)
  }

  getBlogPostByUrlHandle(urlHandle: string): Observable<BlogPost> {
    return this.http.get<BlogPost>(`${environment.apiBaseUrl}/api/BlogPost/${urlHandle}`)
  }

  updateBlogPost(id: string, updateBlogPostRequest: UpdateBlogPostModel): Observable<BlogPost>{
    return this.http.put<BlogPost>(`${environment.apiBaseUrl}/api/BlogPost/${id}?addAuth=true`, updateBlogPostRequest);
  }

  deleteBlogPost(id: string): Observable<BlogPost>{
    return this.http.delete<BlogPost>(`${environment.apiBaseUrl}/api/BlogPost/${id}?addAuth=true`);
  }

}
