import { Component } from '@angular/core';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { Toast } from 'primeng/toast';
import { Body } from './presentation/body/body';
import { Footer } from './presentation/footer/footer';
import { Header } from './presentation/header/header';

@Component({
  selector: 'app-root',
  imports: [Header, Body, Footer, Toast, ConfirmDialog],
  templateUrl: './app.html'
})
export class App {}
