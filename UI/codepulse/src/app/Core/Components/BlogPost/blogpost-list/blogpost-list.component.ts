import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BlogPostService } from '../../../../Services/blog-post.service';
import { Observable } from 'rxjs';
import { BlogPost } from '../../../../Models/BlogPost/blog-post.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-blogpost-list',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './blogpost-list.component.html',
  styleUrl: './blogpost-list.component.css'
})
export class BlogpostListComponent implements OnInit {
  blogPosts$: Observable<BlogPost[]> | undefined;

  constructor(private blogPostService: BlogPostService){
    
  }

  ngOnInit(): void {
    this.blogPosts$ = this.blogPostService.getAllBlogPosts();
  }

}

