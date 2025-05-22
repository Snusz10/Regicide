import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Category } from '../../../../Models/Category/category.model';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { CategoryService } from '../../../../Services/category.service';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css'
})
export class CategoryListComponent implements OnInit {
  categories$: Observable<Category[]> | undefined;

  constructor(private categoryService: CategoryService){
    
  }

  ngOnInit(): void {
    this.categories$ = this.categoryService.getAllCategories();
  }

}
