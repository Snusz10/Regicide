import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { BlogPostService } from '../../../../Services/blog-post.service';
import { BlogPost } from '../../../../Models/BlogPost/blog-post.model';
import { FormsModule } from '@angular/forms';
import * as ngxMarkdown from 'ngx-markdown';
import { CommonModule, DatePipe } from '@angular/common';
import { CategoryService } from '../../../../Services/category.service';
import { Category } from '../../../../Models/Category/category.model';
import { UpdateBlogPostModel } from '../../../../Models/BlogPost/update-blog-post.model';
import { ImageSelectorComponent } from "../../ImageSelector/image-selector.component";
import { ImageService } from '../../../../Services/image.service';

@Component({
  selector: 'app-edit-blogpost',
  standalone: true,
  imports: [FormsModule, DatePipe, ngxMarkdown.MarkdownModule, ngxMarkdown.MarkdownComponent, CommonModule, ImageSelectorComponent],
  templateUrl: './edit-blogpost.component.html',
  styleUrl: './edit-blogpost.component.css'
})
export class EditBlogpostComponent implements OnInit, OnDestroy{
    
    categories?: Observable<Category[]>;
    selectedCategories?: string[];
    id: string | null = null;
    blogPost?: BlogPost;
    routeSubscription?: Subscription;
    blogPostRetrievalSubscription?: Subscription;
    blogPostUpdateSubscription?: Subscription;
    blogPostDeleteSubscription?: Subscription;
    imageSelectSubscription?: Subscription;
    imageSelectorVisible: boolean = false;

    constructor(private route: ActivatedRoute,
            private blogPostService: BlogPostService,
            private categoryService: CategoryService,
            private router : Router,
            private imageService: ImageService){}

    openImageSelector(): void{
        this.imageSelectorVisible = true;
    }

    closeImageSelector(): void{
        this.imageSelectorVisible = false;
    }

    ngOnInit(): void {
        this.categories = this.categoryService.getAllCategories();

        this.routeSubscription = this.route.paramMap.subscribe({
            next: (response) => {
                this.id = response.get('id');

                if (this.id){
                    this.blogPostService.getBlogPostByID(this.id).subscribe({
                        next: (response) => {
                            this.blogPost = response;
                            this.selectedCategories = response.categories.map(x => x.id);
                        }
                    });
                }
            }
        });

        this.imageSelectSubscription = this.imageService.onSelectImage().subscribe({
            next: (response) =>{
                if (this.blogPost){
                    this.blogPost.featuredImageUrl = response.url;
                    this.closeImageSelector();
                }
            }
        });
    }

    onFormSubmit(): void{
        if (!this.blogPost || !this.id || !this.categories){
            console.log("Error when updating the blog post");
            return;
        }
        var updateBlogPost: UpdateBlogPostModel = {
            author: this.blogPost.author,
            content: this.blogPost.content,
            shortDescription: this.blogPost.shortDescription,
            featuredImageUrl: this.blogPost.featuredImageUrl,
            isVisible: this.blogPost.isVisible,
            publishedDate: this.blogPost.publishedDate,
            title: this.blogPost.title,
            urlHandle: this.blogPost.urlHandle,
            categories: this.selectedCategories,
            id: this.id
        };

        this.blogPostUpdateSubscription = this.blogPostService.updateBlogPost(this.blogPost?.id, updateBlogPost).subscribe({
            next: (response) => {
                this.router.navigateByUrl('/admin/blogposts');
            },
            error(response) {
                console.log("Error when updating the blog post");
            }
        });
    }

    onDelete(): void{
        if (this.id == null){
            console.log("Error when updating the blog post");
            return;
        }

        this.blogPostDeleteSubscription = this.blogPostService.deleteBlogPost(this.id).subscribe({
            next: (response) => {
                this.router.navigateByUrl('/admin/blogposts');
            },
            error(response) {
                console.log("Error when deleting the blog post");
            }
        });
    }

    ngOnDestroy(): void {
        this.routeSubscription?.unsubscribe();
        this.blogPostRetrievalSubscription?.unsubscribe();
        this.blogPostUpdateSubscription?.unsubscribe();
        this.blogPostDeleteSubscription?.unsubscribe();
        this.imageSelectSubscription?.unsubscribe();
    }

}
