---
title: 'Catel 4.1'
---
This guide describes how to update your code to be fully compatible with Catel 4.1.

This guide assumes that you are coming from Catel 4.0. If not, please read that guide first.

## IUIVisualizerService

We have reverted the change to force you to use async code on the *IUIVisualizerService*. There are now 2 options:

1.  Synchronous:

    ```
    var result = uiVisualizerService.ShowDialog<MyViewModel>();

    // Window is closed here (synchronous behavior)
    ```

2.  Asynchronous:

    ```
    var result = await uiVisualizerService.ShowDialogAsync<MyViewModel>();

    // Window is closed here thanks to the await keyword
    ```



