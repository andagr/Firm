#About
Firm is a blog-aware, static site generator written in [F#](http://fsharp.org).

#Features
It supports stand-alone pages, blog posts, rss, tags (tag cloud) and post archive. Templates are written in [Razor](https://github.com/Antaris/RazorEngine). Posts and pages are written in [Markdown](http://daringfireball.net/projects/markdown/).

#Getting started
There are quite a few manual steps right now, the plan is to automate this bit by bit with [FAKE](https://github.com/fsharp/FAKE).

1. Fork the project at https://github.com/andagr/Firm.
2. Clone it to a local directory.
3. Create the folders `data\templates\razor` and add the following template files:

        archive.cshtml
        index.cshtml
        page.cshtml
        post.cshtml
        _layout.cshtml

4. The names should hint at what they should do, and if you need some inspiration then look in the [data branch of this repo](https://github.com/andagr/Firm/tree/data).

##Creating your first blog post
1. To write your first blog post, create the directory `data\input\blog\post\<name-of-blog-post>` and add the following files:

    	index.md
    	meta.json

2. Open meta.json and add data about the post, example:

        {
            "title": "Hello",
            "date": "2015-05-28 21:14:00",
            "tags": [ "blog", "hello" ]
        }

3. Open `index.md` in your favorite Markdown editor and write a post.
4. Open `config.json` in the Firm root directory and change `baseUrl` to `http://locahost:8080`. Also take this moment and change the other settings to your liking.
5. Open a console in the root directory of Firm and type: `firm generate`. Dependencies should now be downloaded, project built and web site generated into the folder `output`.
6. In the console, run `firm preview`, verify that the site looks good and then hit `[Enter]` in the console to exit preview mode.
7. Open `config.json` again and change `baseUrl` to the root url of your site, if it's on GitHub Pages (see publishing below) then it's most likely http://(user).github.io/Firm.
8. Open a console in the root directory of Firm and run `firm regenerate`. Please note that it's `regenerate`, not `generate`, Firm will only generate new content for the latter command, and we want to generate everything again to make sure that url's are updated correctly.
9. **Don't forget to add, commit and push the changes to your repository.**
	* The `data` folder is ignored by default, you can open `.gitignore` and remove the entry, it's at the very bottom of the file.
	* The `output` folder is also ignored by default, you can either remove the entry or follow the instructions for publishing to GitHub Pages below.

##Publishing to GitHub Pages
There are [many different hosts for static websites](https://www.google.com/search?q=static+website+hosting), but in this guide I will refer to [GitHub Pages](https://pages.github.com/). Please note that in this case I refer to GitHub Pages for project sites.

###One time setup
1. Open a console in the root directory of Firm and create a new branch:

        git branch gh-pages
        git checkout gh-pages

2. Delete everything except the `output` directory.
3. Move the content of the `output` directory to the root directory of Firm.
4. Delete the now empty `output` directory.
5. Stage, commit and publish/push the changes:

        git add -A *
        git commit -m "Created first blog post!"
        git push --set-upstream origin gh-pages

6. Switch back to the master branch:

        git checkout master

7. Now here's the trick, we want to store the `gh-pages` branch in the `output` directory, so that when we create a new post we can simply move into that directory and push the changes directly to the correct branch. In a console in the root directory of Firm, clone the `gh-pages` branch into the `output` directory:

        git clone -b gh-pages https://github.com/(user)/Firm.git output

###Creating a repeatable workflow

1. Add or edit a post or page.
2. Change config.json:baseUrl to http://localhost:8080.
3. Run `firm regenerate` to generate your site in the `output` folder.
4. Run `firm preview` and verify that it looks ok.
5. Change back config.json.
6. Run `firm regenerate` again.
6. Add, commit and push changes on both your `master` and `gh-pages` branches.

You're done! All you have to do from now on is create content, verify that it looks ok and then push it to GitHub Pages.

Happy blogging!

#Thanks to

* [FsBlog](https://github.com/fsprojects/FsBlog) - For the idea.
* [FSharp.Formatting](https://github.com/tpetricek/FSharp.Formatting) - Markdown parsing and code formatting.
* [RazorEngine](https://github.com/Antaris/RazorEngine) - Templates.
* [FAKE](https://github.com/fsharp/FAKE) - Building and general tool.
* [FSharp.Data](https://github.com/fsharp/FSharp.Data) - JSON parsing.
