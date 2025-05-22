These templates use the "Power Header" extension to automatically output our header to any newly created file in the project.
You can make changes to the header(s) as necessary.

For files with text already in them -> press "Ctrl + Alt + H" to insert the header comment

A good resource for values that will be inserted upon file creation are shown here:

https://code.visualstudio.com/docs/editing/userdefinedsnippets#_variables


If you would like to apply different headers to different files types you can create an new header
in this folder, and then apply it to your file type by adding the below statement to the "settings.json" file

"[plaintext]":{ // file type
    "powerHeader.autoInsert.enable": true,
    "powerHeader.autoInsert.allow": "always",
    "powerHeader.commentMode": "raw",
    // where to derive the template from
    "powerHeader.template": "file://$WORKSPACE_FOLDER/src/templates/typescriptHeaderComment.txt"
}