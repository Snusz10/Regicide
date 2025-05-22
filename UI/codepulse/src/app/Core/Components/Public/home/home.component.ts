import { Component, OnInit } from '@angular/core';
import { BlogPostService } from '../../../../Services/blog-post.service';
import { Observable } from 'rxjs';
import { BlogPost } from '../../../../Models/BlogPost/blog-post.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{

    blogs?: Observable<BlogPost[]>;

    constructor(private blogPostService: BlogPostService){
        
    }


    ngOnInit(): void {
        this.blogs = this.blogPostService.getAllBlogPosts();
    }


}
