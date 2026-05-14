// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\external-components\pixdinn-html-editor\pixdinn-html-editor.component.ts
import { Renderer2 } from '@angular/core';
import { Component, OnDestroy, OnInit, ViewChild, AfterViewInit, ElementRef } from '@angular/core';
import { Editor, Toolbar } from "ngx-editor";

@Component({
  selector: 'PixdinnHtmlEditor',
  templateUrl: './pixdinn-html-editor.component.html',
  styleUrls: ['./pixdinn-html-editor.component.scss']
})
export class PixdinnHtmlEditorComponent implements OnInit, OnDestroy, AfterViewInit {

    @ViewChild("editorMenu", { read: ElementRef }) editorMenu: ElementRef;
    @ViewChild("editorElement", { read: ElementRef }) editorElement: ElementRef;

    editordoc = "";
    editor: Editor;
    toolbar: Toolbar = [
        ["bold", "italic"],
        ["underline", "strike"],
        ["code", "blockquote"],
        ["ordered_list", "bullet_list"],
        [{ heading: ["h1", "h2", "h3", "h4", "h5", "h6"] }],
        ["link", "image"],
        ["text_color", "background_color"],
        ["align_left", "align_center", "align_right", "align_justify"],
        []
    ];
    colorPresets = ['red', '#FF0000', 'rgb(255, 0, 0)'];

    constructor(private renderer: Renderer2) { }

    ngOnInit(): void {
        this.editor = new Editor();
    }

    ngOnDestroy(): void {
        this.editor.destroy();
    }

    ngAfterViewInit(): void {
        this.renderer.setStyle(this.editorElement.nativeElement, "height", 800);
    }
}
