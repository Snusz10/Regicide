import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Category } from '../../../../Models/Category/category.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UpdateCategoryModel } from '../../../../Models/Category/update-category-request.model';
import { CategoryService } from '../../../../Services/category.service';

@Component({
  selector: 'app-edit-category',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-category.component.html',
  styleUrl: './edit-category.component.css'
})
export class EditCategoryComponent implements OnInit, OnDestroy{
  
  id : string | null = null;
  category? : Category;
  paramsSubscription?: Subscription;
  updateSubscription?: Subscription;
  deleteSubscription?: Subscription;
  
  constructor(private route: ActivatedRoute,
    private categoryService: CategoryService,
    private router: Router){

  }

  onFormSubmit(): void{
    const updateCategoryModel: UpdateCategoryModel = {
      // if the category is null, use an empty string instead
      // `??` = if null 
      name: this.category?.name ?? '',
      urlHandle: this.category?.urlHandle ?? ''
    };

    if (this.id){
      this.updateSubscription = this.categoryService.updateCategory(this.id, updateCategoryModel)
      .subscribe({
        next: () => {
          this.router.navigateByUrl('/admin/categories');
        },
        error: () => {
          console.log('You encountered an error when updating!');
        }
      });
    }
  }

  onDelete(): void{
    console.log("deleting");
    if (this.id){
      this.deleteSubscription = this.categoryService.deleteCategory(this.id)
      .subscribe({
        next: () => {
          this.router.navigateByUrl('/admin/categories');
        },
        error: () => {
          console.log('You encountered an error when deleting!');
        }
      });
    }
  }

  ngOnInit(): void {
    this.paramsSubscription = this.route.paramMap.subscribe({
      next: (params) => {
        this.id = params.get('id');

        if (this.id){
          this.categoryService.getCategoryByID(this.id).subscribe({
            next: (response: Category | undefined) => {
              console.log("Retrieved result")
              this.category = response;
            }
          });
        }
      },

      
      error: (params) => {
        console.log("There was an error getting the ID when trying to edit a category");
      }
    });
  }
  
  ngOnDestroy(): void {
    this.paramsSubscription?.unsubscribe();
    this.updateSubscription?.unsubscribe();
    this.deleteSubscription?.unsubscribe();
  }

}
