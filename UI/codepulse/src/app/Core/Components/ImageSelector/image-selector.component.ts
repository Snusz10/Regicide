import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ImageService } from '../../../Services/image.service';
import { Observable, Subscription } from 'rxjs';
import { Image } from '../../../Models/Image/image.model';

@Component({
  selector: 'app-image-selector',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './image-selector.component.html',
  styleUrl: './image-selector.component.css'
})
export class ImageSelectorComponent implements OnInit, OnDestroy {

    private imageUploadSubscription?: Subscription;
    private imageRetrieveSubscription?: Subscription;
    protected images?: Observable<Image[]>;

    @ViewChild('form', {static: false}) imageUploadForm?: NgForm;

    constructor(private imageService: ImageService){}
    
    ngOnInit(): void {
        this.getImages();

    }

    private file?: File;
    protected fileName: string = '';
    protected title: string = '';

    onFileUploadChange(event: Event): void{
        const element = event.currentTarget as HTMLInputElement;
        this.file = element.files?.[0];
    }

    uploadButtonPressed():void{
        if (this.file && this.fileName !== '' && this.title != ''){
            this.imageUploadSubscription = this.imageService.uploadImage(this.file, this.fileName, this.title).subscribe({
                next: (response) => {
                    this.imageUploadForm?.resetForm();
                    this.getImages();
                }
            })
        }
    }

    ngOnDestroy(): void {
        this.imageUploadSubscription?.unsubscribe();
        this.imageRetrieveSubscription?.unsubscribe();
    }

    private getImages(): void{
        this.images = this.imageService.getAll();
    }

    protected imagePressed(image: Image): void{
        this.imageService.selectImage(image);
    }
}
