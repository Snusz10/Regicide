import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BlogPostService } from '../../../../Services/blog-post.service';
import { BlogPost } from '../../../../Models/BlogPost/blog-post.model';
import { Observable } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MarkdownModule } from 'ngx-markdown';

@Component({
  selector: 'app-blog-details',
  standalone: true,
  imports: [FormsModule, CommonModule, MarkdownModule],
  templateUrl: './blog-details.component.html',
  styleUrl: './blog-details.component.css'
})
export class BlogDetailsComponent implements OnInit {

    private url: string | null = null;
    protected blogPost? : Observable<BlogPost>;
    

    constructor(private route: ActivatedRoute,
                private blogPostService: BlogPostService
    ){}

    ngOnInit(): void {
        this.route.paramMap.subscribe({
            next: (response) =>{
                this.url = response.get('url');
            }
        });

        if (this.url){
            this.blogPost = this.blogPostService.getBlogPostByUrlHandle(this.url);
        }
        
    }



}
