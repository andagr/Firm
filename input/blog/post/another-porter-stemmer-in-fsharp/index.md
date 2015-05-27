Stemming
--------

Stemming is the process for reducing words to their root form, e.g. both "acceptable" and "acceptance" might be reduced to "accept".

I'm working on a side project where I will have use for a stemmer, so I decided to look around a bit for a straight forward, and well explained, solution. Many resources seemed to point towards the [Porter Stemmer][1]. This is an algorithm created by [Martin Porter][2] and it works by setting up a few rules which are then, together with matching word endings, used for matching which words should be reduced to which stems.

Please note that as the title suggests, this is hardly the only F# solution. A search quickly reveals at least two others:

* [A Porter Stemmer in F# - MSDN Blogs][3]
* [Porter Stemmer in F# | Faisal's space][4]

Using the algorithm description, and by getting quite a lot of inspiration from Faisal's solution, I have put together my own implementation:

Type
----

The only type in this implementation, it denotes either vowel or consonant:

    type private Kind =
        | V
        | C

Base Functions
----------------

Here are a few helper functions which helps with converting a word to a list of vowels/consonants, group them so that e.g. VCCVC becomes VCVC and finally get the measurement of a word. The measurement is calculated by counting the number of VC pairs.

    let private (|BaseV|_|) char =
        match char with
        | 'a' | 'e' | 'i' | 'o' | 'u' -> Some BaseV
        | _ -> None
    
    let private kinds (word:string) =
        let rec kinds chars =
            match chars with
            | [] -> []
            | ['y'] -> [C]
            | 'y'::t -> (kinds t |> List.head |> (function | V -> C | C -> V))::kinds t
            | BaseV::t -> V::kinds t
            | _::t -> C::kinds t
        word |> Seq.toList |> List.rev |> kinds |> List.rev
    
    let private pack kinds =
        kinds
        |> List.fold
            (fun kl k -> 
                match k::kl with 
                | [] -> [k]
                | V::V::_ | C::C::_ -> kl
                | _ -> k::kl) []
        |> List.rev
    
    let rec private measurement kinds =
        match kinds with
        | [] -> 0
        | V::C::t -> (measurement t) + 1
        | h::t -> measurement t

Conditions and Rules
--------------------

Following this is a couple of functions that does the matching against the conditions and word suffixes as specified by the algorithm.

    /// The stem ends with e.g. "s" or any letter/word. (*S in the Porter algorithm description.)
    let private (|Ends|_|) (s:string list) (word:string) =
        match List.tryFind (fun s -> word.EndsWith(s)) s with
        | Some s -> Some ((word.Substring(0, String.length word - String.length s)), s)
        | None -> None
    
    let private ends s trunk =
        match trunk with
        | Ends s _ -> true
        | _ -> false
    
    /// The stem ends with a double -and equal- consonant. (*d in the Porter algorithm description.)
    let private (|EndsDoubleC|_|) trunk =
        match trunk |> kinds |> List.rev with
        | C::C::_ when trunk.[String.length trunk - 2] = trunk.[String.length trunk - 1] -> Some ((trunk.Substring(0, String.length trunk - 2)), (trunk.Substring(String.length trunk - 2, 2)))
        | _ -> None
    
    /// Calculates the measurement of a stem. (m in the Porter algorithm description.)
    let private m  =
        kinds >> pack >> measurement
    
    /// The stem contains a vowel. (*v* in Porter algorithm description.)
    let private hasVowel trunk =
        trunk |> kinds |> List.exists (fun k -> k = V)
    
    /// The word ends in CVC, where the second C (i.e. the last character) is not w, x or y. (*o in the Porter algorithm description.)
    let private (|EndsCVCNotWXY|_|) word =
        match word with
        | Ends ["w"; "x"; "y"] _ -> None
        | t ->
            match t |> kinds |> List.rev with
            | C::V::C::_ -> Some t
            | _ -> None
    
    let private notEndsCVCNotWXY trunk =
        match trunk with
        | EndsCVCNotWXY _ -> false
        | _ -> true

Steps
-----

And finally, the steps that the words flow through and a function that composes them in the right order. The steps contains the specific conditions and word suffixes that must be matched for a change to be made.

    let private step1a w =
        match w with
        | Ends ["sses"; "ss"] (t, s) -> t + "ss"
        | Ends ["ies"] (t, s) -> t + "i"
        | Ends ["s"] (t, s) -> t
        | _ -> w
    
    let private step1b w =
        let step1bX w =
            match w with
            | Ends ["at"] (t, s) -> t + "ate"
            | Ends ["bl"] (t, s) -> t + "ble"
            | Ends ["iz"] (t, s) -> t + "ize"
            | EndsDoubleC (t, s) when not (s = "ll" || s = "ss" || s = "zz")  -> t + string (Seq.head s)
            | EndsCVCNotWXY t when m t = 1  -> t + "e"
            | _ -> w
        match w with
        | Ends ["eed"] (t, s) -> if m t > 0 then t + "ee" else t + s
        | Ends ["ed"; "ing"] (t, s) when hasVowel t  -> step1bX t
        | _ -> w
    
    let private step1c w =
        match w with
        | Ends ["y"] (t, s) when hasVowel t -> t + "i"
        | _ -> w
    
    let private step2 w =
        match w with
        | Ends ["ational"] (t, s) when m t > 0 -> t + "ate"
        | Ends ["fulness"] (t, s) when m t > 0 -> t + "ful"
        | Ends ["iveness"] (t, s) when m t > 0 -> t + "ive"
        | Ends ["ization"] (t, s) when m t > 0 -> t + "ize"
        | Ends ["ousness"] (t, s) when m t > 0 -> t + "ous"
        | Ends ["biliti"] (t, s) when m t > 0 -> t + "ble"
        | Ends ["tional"] (t, s) when m t > 0 -> t + "tion"
        | Ends ["alism"; "aliti"] (t, s) when m t > 0 -> t + "al"
        | Ends ["ation"] (t, s) when m t > 0 -> t + "ate"
        | Ends ["entli"] (t, s) when m t > 0 -> t + "ent"
        | Ends ["iviti"] (t, s) when m t > 0 -> t + "ive"
        | Ends ["ousli"] (t, s) when m t > 0 -> t + "ous"
        | Ends ["abli"] (t, s) when m t > 0 -> t + "able"
        | Ends ["alli"] (t, s) when m t > 0 -> t + "al"
        | Ends ["anci"] (t, s) when m t > 0 -> t + "ance"
        | Ends ["ator"] (t, s) when m t > 0 -> t + "ate"
        | Ends ["enci"] (t, s) when m t > 0 -> t + "ence"
        | Ends ["izer"] (t, s) when m t > 0 -> t + "ize"
        | Ends ["eli"] (t, s) when m t > 0 -> t + "e"
        | _ -> w
    
    let private step3 w =
        match w with
        | Ends ["alize"] (t, s) when m t > 0 -> t + "al"
        | Ends ["ative"] (t, s) when m t > 0 -> t
        | Ends ["icate"; "iciti"; "ical"] (t, s) when m t > 0 -> t + "ic"
        | Ends ["ness"; "ful"] (t, s) when m t > 0 -> t
        | _ -> w
    
    let private step4 w =
        match w with
        | Ends ["al"; "ance"; "ence"; "er"; "ic"; "able"; "ible"; "ant"; "ement"; "ment"; "ent"; "ou"; "ism"; "ate"; "iti"; "ous"; "ive"; "ize"] (t, s) when m t > 1 -> t
        | Ends ["ion"] (t, s) when m t > 1 && ends ["s"; "t"] t -> t
        | _ -> w
    
    let private step5a w =
        match w with
        | Ends ["e"] (t, s) when m t > 1 -> t
        | Ends ["e"] (t, s) when m t = 1 && notEndsCVCNotWXY t -> t
        | _ -> w
    
    let private step5b w =
        match w with
        | EndsDoubleC (t, s) when m w > 1 && s = "ll" -> t + string (Seq.head s)
        | _ -> w
    
    let stem word =
        word |> step1a |> step1b |> step1c |> step2 |> step3 |> step4 |> step5a |> step5b

Tests
-----

I have tested the implementation successfully against the [list of vocabularies and their stemmed equivalents][5] that is linked to from the original algorithm description page.

Source Code
-----------

The full solution can be downloaded from [github][6].

Conclusion
----------

In the end I'm pretty happy with the result, I've tried to make it as easy as possible to read and I hope I have reached that goal. I feel that F# has let me translate the description of the original algorithm very close to the code equivalent, much thanks to the amazing pattern matching that the language has to offer.


  [1]: http://snowball.tartarus.org/algorithms/porter/stemmer.html
  [2]: http://tartarus.org/~martin/
  [3]: http://blogs.msdn.com/b/christianb/archive/2011/06/24/a-porter-stemmer-in-f.aspx
  [4]: http://fwaris.wordpress.com/2012/10/30/porter-stemmer-in-f/
  [5]: http://snowball.tartarus.org/algorithms/porter/diffs.txt
  [6]: https://github.com/andagr/PorterStemmer