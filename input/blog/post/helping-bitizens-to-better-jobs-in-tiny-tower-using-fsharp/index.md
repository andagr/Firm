![Tiny Tower][1]

From the [Tiny Tower wiki][2]:

> Tiny Tower is a simulation game developed by [NimbleBit][3] and released in
> June 2011. It is available to download on Apple iOS devices (3.0 or
> later) and Android devices. The object of the game is to build and
> manage a large skyscraper. Each new floor the player builds is either
> a Commercial floor that hosts businesses and venues, or a Residential
> floor that Bitizens live in. Bitizens live and work in the player's
> Tower, paying rent every day and stocking the Commercial floors with
> items to sell. The player's goal is to turn a profit, build more
> floors, and manage their ever-growing tower.

Like you and me, bitizens have job satisfaction. This is decided by two things, their skill in the job, and if it is their dream job. To be specific:

Category / Color
------------

Each business has a color that represents a category:

* Green for food
* Blue for service
* Purple for retail
* Yellow for recreation
* Orange for creative

Each bitizen has one skill level, ranging from 0 - 9, in each color. The higher the skill level, the greater the discount will be for stocking items.

Dream Job
---------

They also have one dream job that corresponds to a name of a business. When a bitizen is employed at her dream job she will be able to stock twice as many items as normal.

Sorting by Job Satisfaction
---------------------------

Higher job satisfaction means higher revenue, and using the above information we can create a tool that takes a list of bitizens and a list of businesses as input, and outputs a list of optimal employments (each business can employ three bitizens).

Let's use the following formula for scoring the employment value of a bitizen:

    Skill * 10 + 10 - Avg(Rest of Skills)

What this means is that the skill we are looking for is the one highest valued, and that specialists (high skill in question, low for others) are more valued than generalists (high in all skills). Here are some examples:

      |
      V
    0,9,0 => 9*10 + 10-0 = 100
    1,9,1 => 9*10 + 10-1 = 99
    8,9,1 => 9*10 + 10-4.5 = 95,5
    8,9,8 => 9*10 + 10-8 = 92
    9,9,9 => 9*10 + 10-9 = 91
    0,8,0 => 8*10 + 10-0 = 90

The arrow denotes the skill column we are looking for in this specific example (the actual data will have of course have 5 skill values for each bitizen). As you can see, a bitizen with 9 in all skills is valued lower than one with 9 in only the desired skill. This is because if there are, say, 4 bitizens with a skill value of 9 for a specific job, then it's probably better to use up the specialist for this job and save the generalist for another job further down the list. The calculated value is then used to sort the list of bitizens based on which business that are looking for employees.

Types
-----
Let's start with the F# types:

    type Color =
        | Green
        | Blue
        | Yellow
        | Purple
        | Orange
        with
            static member Parse color =
                match color with
                | "green" -> Color.Green
                | "blue" -> Color.Blue
                | "yellow" -> Color.Yellow
                | "purple" -> Color.Purple
                | "orange" -> Color.Orange
                | _ -> raise(ArgumentException("Invalid color"))
            override this.ToString() =
                match this with
                | Green -> "Green"
                | Blue -> "Blue"
                | Yellow -> "Yellow"
                | Purple -> "Purple"
                | Orange -> "Orange"

    type Skill(color: Color, value: int) =
        do if value < 0 || value > 9 then raise (ArgumentException("Invalid skill value: " + sprintf "%i" value))
        member this.Color = color
        member this.Value = value

    type Bitizen(name: string, dreamJob: string, skills: Skill[]) =
        do if skills.Length <> 5 then raise(ArgumentException("Invalid number of skills"))
        let skillSum = skills |> Array.sumBy (fun s -> s.Value) |> decimal
        member this.Name = name
        member this.DreamJob = dreamJob
        member this.SortValueFor (color: Color) =
            let matchingSkill = decimal (skills |> (Array.find (fun s -> s.Color = color))).Value
            let avgOtherskills = (skillSum - matchingSkill) / 4m
            0m - (matchingSkill * 10m + 10m - avgOtherskills)
        override this.ToString() =
            name + ", " + dreamJob + ", [| " + String.Join("; ", skills |> Array.map (fun s -> string s.Color + ": " + string s.Value)) + " |]"

    type Job = 
        { Name: string; Color: Color; }
        with
            override this.ToString() =
                this.Name + ", " + string this.Color

    type Position = 
        { Job: Job; Employee: Bitizen; }
        with
            override this.ToString() =
                let jobStr = string this.Job
                jobStr + String.replicate (25 - jobStr.Length) " " + string this.Employee

From this we can create a list of bitizens that each have a dream job and a helper method for getting her skill value based on a color. We can also create a list of jobs where each job is of a specific color (i.e. category).

Filling vacant positions
------------------------
From one list of jobs and one of bitizens, assign jobs according to our earlier defined formula using the following function:

    let fillPositions filter (jobs: Job list) (bitizens: Bitizen list) =
        let fillOne (job: Job) (bitizens: Bitizen list) filter =
            let matches, rest = List.partition (fun b -> filter job b) bitizens
            let sortedMatches = matches |> List.sortBy (fun b -> b.SortValueFor job.Color)
            match sortedMatches with
            | [] -> None, bitizens
            | head::tail  -> Some { Job = job; Employee = head }, tail@rest
        let rec fillPositionsInner (jobs: Job list) (bitizens: Bitizen list) (positions: Position list) =
            match jobs with
            | [] -> positions, bitizens
            | head::tail -> match fillOne head bitizens filter with
                            | None, b -> fillPositionsInner tail b (positions)
                            | Some p, b -> fillPositionsInner tail b (p::positions)
        fillPositionsInner jobs bitizens []

This function contains two other functions, `fillOne` and `fillPositionsInner`. The latter takes the first job and then uses `fillOne` to find the most suitable bitizen. Apart from the jobs- and bitizen lists a filter argument is also required, more on this soon. `fillOne` sorts the bitizen list according to "best for the requested skill", if the bitizen list is empty then a tuple `None * Bitizen list` is returned, otherwise `Some(Position) * Bitizen list`, where the second position holds the remaining unassigned bitizens, is returned.

Program Start
-------------

The program is started using this function:

    [<EntryPoint>]
    let main argv =
        let jobs = 
            (new CsvProvider<"shops.csv">()).Data
            |> Seq.map (fun r -> seq { for i in 1..3 -> { Name = r.Name; Color = Color.Parse r.Color }})
            |> Seq.collect (fun o -> seq { yield! o })
            |> List.ofSeq
    
        let bitizens = 
            (new CsvProvider<"bitizens.csv">()).Data
            |> Seq.map
                (fun r ->
                    Bitizen(r.Name, r.DreamJob, 
                        [| 
                            Skill(Color.Blue, r.Blue);
                            Skill(Color.Green, r.Green);
                            Skill(Color.Orange, r.Orange);
                            Skill(Color.Purple, r.Purple);
                            Skill(Color.Yellow, r.Yellow)
                        |]))
            |> List.ofSeq
    
        let dreamPositions, bitizens = fillPositions (fun j b -> j.Name.ciCompare(b.DreamJob)) jobs bitizens
        let normalPositions, bitizens = fillPositions (fun j b -> true) jobs bitizens
    
        File.WriteAllLines("positions.txt", dreamPositions@normalPositions |> List.map (fun p -> string p))
    
        0

It reads two csv files using the [csv type provider][5] and from this creates the types we need (oh, and type providers are the greatest thing ever). The available jobs list is created by making three jobs for each business. Next we need to fill positions by calling the `fillPositions` function. This is done twice, first by using a filter function that matches dream jobs, making sure that these are assigned first, then once more for the remaining bitizens. One last note on this function, as you can see there is a string method `ciCompare` in the dream positions filter function. This is simply an extension function on the string type which is defined as:

    type String with
        member this.ciCompare other =
            System.String.Equals(this, other, StringComparison.InvariantCultureIgnoreCase)

Source
------

You can find the full source as a Visual Studio solution on my github account: [https://github.com/andagr/SortTinyTowerWorkers][6]

Final Note
-------------

I wrote this tool as an F# learning experience and I welcome any input on improvements on everything from the syntax to how this could be better modeled.


  [1]: /blog/post/helping-bitizens-to-better-jobs-in-tiny-tower-using-fsharp/img/650px-Banner.png
  [2]: http://tinytower.wikia.com/wiki/Tiny_Tower_Wiki
  [3]: http://www.nimblebit.com/
  [5]: http://fsharp.github.io/FSharp.Data/library/CsvProvider.html
  [6]: https://github.com/andagr/SortTinyTowerWorkers