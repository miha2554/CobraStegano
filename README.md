# CobraStegano
Graduation project: software implementation for image steganography using the least significant bit method.

This program has two lines of defense: first, there is the password. 
The application hides the information not in all of the least significant bits in a row, 
but only in those defined by our function, which takes our password as its argument. 

Second, before hiding the information inside the image, the application encrypts it. 

How does this work in a docx document?

A document containing textual information is downloaded, this document is converted to XML code using the .NET UseOffice component, 
then this code is encrypted using the AES algorithm.
And then the encrypted XML code is injected into the image bits.
