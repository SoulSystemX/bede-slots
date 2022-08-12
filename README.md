# Bede Slots - By Lee Maguire

## How to run 

Ensure docker is installed on your machine firstly.

### Vs Code - Backend setup 

Firstly you should open the folder in which the `Bede-slots` folder exists on vscode, this contains the dotnet project and the tests project. 

![Pasted image 20220812145214.png](/static/Pasted%20image%2020220812145214.png)

Select the open remote windows button from the bottom corner, the green button with two arrows:
![Pasted image 20220812142700.png](/static/Pasted%20image%2020220812142700.png)

Choose `reopen in container` from the top
![Pasted image 20220812142844.png](/static/Pasted%20image%2020220812142844.png)

Select `From a predefined container`

![Pasted image 20220812143430.png](/static/Pasted%20image%2020220812143430.png)
select `c# (.NET)`
![Pasted image 20220812143509.png](/static/Pasted%20image%2020220812143509.png)

Then choose all the defaults and no additional tools

When it loads select yes here:
![[Pasted image 20220812143600.png]]

Then in the console type `cd bede-slots` then type  `dotnet run` to start the backend. You may also use the run and debug menu from the side bar if you want to step through code.
![Pasted image 20220812143755.png](/static/Pasted%20image%2020220812143755.png)

You can access the openapi / swagger page at http://localhost:5230/swagger

### Running tests
Run the tests by typing from the root folder `cd bede-slots.tests` and then `dotnet test`.


### Bede-slots - frontend setup

Similarly to the backend we can use VS code and docker to run a development container.


open the bede-slot-frontend folder and then the bede-test folder

![Pasted image 20220812144240.png](/static/Pasted%20image%2020220812144240.png)

Same as the backend open a remote window and reopen in container

this time select node.js
![Pasted image 20220812144349.png](/static/Pasted%20image%2020220812144349.png)

Choose all the default options and no additional features and press ok.

Open a terminal in vs code "ctrl + shift + ` "

Type `npm i`

When it completes run `npm run dev`

The terminal will provide a link in which to access the frontend for the slots game