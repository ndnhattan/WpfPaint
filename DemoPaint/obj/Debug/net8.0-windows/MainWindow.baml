   M S B A M L   `   `   `  ����N  JFluent, Version=10.0.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5   	   D @DemoPaint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:DemoPaint	DemoPaint 5   	      DemoPaint.MainWindow   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   7Fluent$clr-namespace:Fluent;assembly=Fluent     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:DemoPaint     5   	   -   5          � Title$  
MainWindow��5	   	   $	��450��6   $	��800��6)   .��5      ]� $
��True��6   .+�5   
      Fluent.Ribbon  -   5          Name$ ribbon��5   -   $	��Top=�6      Menu 5         Fluent.Backstage  5         Fluent.RibbonControl   Header$
 File��6#      Content. 5          Fluent.BackstageTabControl  .@�5         Fluent.BackstageTabItem  -   5           Header$ Save to PNG��5   2   5        5      -   5        $ Load to PNG��5   2   5        5      -   5        $ Load to Binary��5   2   5        5      -   5        $ Save to Binary��5   2   5        5      -   5        $
 Quit��5   2   5      5      5         Tabs. 5         Fluent.RibbonTabItem     Header$
 Edit��6#      Groups. 5         Fluent.RibbonGroupBox  -   5        	  Name$	 RibbonShape��5   7   $��Shape��6(   5        5       -	   5        $	 
RibbonLine��5    6   $
��line��6(   .@�5!      �� -
   5        $��brushesListBox��5!      $
��Left=�65   $	��Top=�6P   $��80��6h   .@�5"      �� 
 �� 
IsSelected$

 True��6'   .��69   Line6?   �� 5#      .��6'   Dashed6/   �� 5$      .��6'   Dotted6/   5%      5'        5(      $
��Size��6(   .@�5*      �� -   5        $��sizesListBox��5*      $
��Left=�63   $	��Top=�6N   $��80��6f   .@�5+      �� $

 True��6'   .��69   1px6>   �� 5,      .��6'   3px6,   �� 5-      .��6'   5px6,   �� 5.      .��6'   8px6,   5/      50        51      $��Color��6(   .@�52      �� -   5        $��colorComboBox��52      $��80��64   .@�53      �� $

 True��6'   $��Black��69   
���   �6I   �� 54      $	��Red��6'   
���  ��65   �� 55      $
��Blue��6'   
����  �66   �� 56      $��Green��6'   
��� � �67   58      59        5:      -   5        $	 btnUndo��5:   6   $
��Undo��6(   .@�5;         Fluent.Button  -   5           Icon$ assets/undo.png��5;   $    �� BitmapScalingMode$ HighQuality=�6;   5<      5=        5>      -   5        $	 btnRedo��5>   6   $
��Redo��6(   .@�5?        -   5        $ assets/redo.png��5?   $   $ HighQuality=�6;   5@      5A        5B      $
��Tool��6(   .@�5C      �� -   5        $��colorFillComboBox��5C      $��80��68   .@�5D      �� $

 True��6'   $��White��69   
�������6I   �� 5E      $��Black��6'   
���   �67   �� 5F      $	��Red��6'   
���  ��65   �� 5G      $
��Blue��6'   
����  �66   �� 5H      $��Green��6'   
��� � �67   5J      �� 5K      -   5         ��Name$ btnFill��5K      $
��Fill��6D   $��80��6S   $��0,5,0,0q�6^   5L        5M      $
��Text��6(   .@�5N      �� -   5        $��	textInput��5N      $	��200��6/   $��30��6;   $��
Enter Text��6G   �� 5O      -   5        $��
btnAddText��5O   F   $��Add Text��6   6F   5P      5Q      5R      �� 5S   
   $��0q�6    �� SelectedIndex 6)   ��  �� ElementName$ ribbon�� �� Path$ SelectedTabIndex� � 	Resources 5T       �� 
TargetType �� Property �� Value �� 
TargetType��%{   '��       �� 5U      " ��6   .�5V      ��  � �� 5W      �� 5X      " ��6.   5Y      5[      5\      5]      .@�5^      �� .��5_      � -   5        $
}�True��5_   0    � RowDefinitions 5`      �� 5a      $��* �6(   .+�5c      �� -   5        $��actionsStackPanel��5c   !   
���  �� �� Orientation$ 
Horizontal=�6K   5d      �� 5f      -   5        $��drawingCanvas��5f      
�������z�5g      �� 5h      -   5        canvasScaleTransform��5h   )   5j      �� 5k      -   5         ��Name$ Canvas��5k      $��1��6)   
������ 5m      5n      5o      5p      5q      5t      