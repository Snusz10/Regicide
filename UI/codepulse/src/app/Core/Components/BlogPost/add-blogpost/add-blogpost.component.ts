import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { BlogPostService } from '../../../../Services/blog-post.service';
import { AddBlogPostModel } from '../../../../Models/BlogPost/add-blog-post.model';
import * as ngxMarkdown from 'ngx-markdown';
import { CategoryService } from '../../../../Services/category.service';
import { Category } from '../../../../Models/Category/category.model';
import { ImageSelectorComponent } from "../../ImageSelector/image-selector.component";
import { ImageService } from '../../../../Services/image.service';

@Component({
  selector: 'app-add-blogpost',
  standalone: true,
  imports: [FormsModule, DatePipe, ngxMarkdown.MarkdownModule, ngxMarkdown.MarkdownComponent, CommonModule, ImageSelectorComponent],
  templateUrl: './add-blogpost.component.html',
  styleUrl: './add-blogpost.component.css'
})
export class AddBlogpostComponent implements OnInit, OnDestroy{
    blogPost: AddBlogPostModel;
    categories$?: Observable<Category[]>;

    private addBlogPostSubscription?: Subscription;
    imageSelectSubscription?: Subscription;
    imageSelectorVisible: boolean = false;

    constructor(private blogPostService: BlogPostService,
            private router: Router,
            private categoryService: CategoryService,
            private imageService: ImageService){
        this.blogPost = {
            title: '',
            shortDescription: '',
            urlHandle: '',
            content: '',
            featuredImageUrl: '',
            author: '',
            isVisible: true,
            publishedDate: new Date(),
            categories: []
        }
    }

    openImageSelector(): void{
        this.imageSelectorVisible = true;
    }

    closeImageSelector(): void{
        this.imageSelectorVisible = false;
    }

    ngOnInit(): void {
        this.categories$ = this.categoryService.getAllCategories();

        this.imageSelectSubscription = this.imageService.onSelectImage().subscribe({
            next: (response) =>{
                if (this.blogPost){
                    this.blogPost.featuredImageUrl = response.url;
                    this.closeImageSelector();
                }
            }
        });
    }

    protected onBlogPostSubmit(): void{
        this.addBlogPostSubscription  = this.blogPostService.addBlogPost(this.blogPost).subscribe({
            next: (successResponse) => {
              this.router.navigateByUrl('/admin/blogposts')
            },
            error: (errorResponse) =>{
              console.log('Adding a blog post was a failure!');
            }
        });
    }

    ngOnDestroy(): void {
        this.addBlogPostSubscription?.unsubscribe();
        this.imageSelectSubscription?.unsubscribe();
    }
}
