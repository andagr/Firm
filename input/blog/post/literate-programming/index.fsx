(**
##What is it?
You can write posts in Firm in [literate programming style](http://tpetricek.github.io/FSharp.Formatting/literate.html).
What that means is that instead of writing documents in markdown text and specifically format the parts that contain code,
you turn it around and make code your main content and embed text as comments.
The major upside of this is that the source of your post or page is an fsharp script file (.fsx) that can be run in e.g.
Visual Studio.

##Made possible by
The fsharp script file is fed to [FSharp.Formatting](http://tpetricek.github.io/FSharp.Formatting),
which does all the heavy lifting.

##Example
Here are some code snippets copied from http://fssnip.net with the solution the first
[Project Euler](https://projecteuler.net/) problem (find the sum of all multiples of 3 or 5 below 1000).

First the range.
*)
let range = [0..999]

(**
Then the predicate that checks for multiples of 3 or 5.
*)
let condition x = x % 3 = 0 || x % 5 = 0

(**
And finally the function that runs the above declared predicate on the range and sums it up using built in library
functions.
*)

range 
|> List.filter condition 
|> List.fold (+) 0

(**
##More examples
You can find an example that covers pretty much all features on the
[FSharp.Formatting documentation site](http://tpetricek.github.io/FSharp.Formatting/sidescript.html).

You can check
[the source of this blog post](https://github.com/andagr/Firm/tree/data/input/blog/post/literate-programming/index.fsx)
to see the actual script.
*)