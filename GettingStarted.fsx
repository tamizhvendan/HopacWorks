#r "packages/Hopac/lib/net45/Hopac.Core.dll"
#r "packages/Hopac/lib/net45/Hopac.Platform.dll"
#r "packages/Hopac/lib/net45/Hopac.dll"

open Hopac


let createJob jobId delayInMillis  = job {
  printfn "starting job #%d" jobId
  do! timeOutMillis delayInMillis
  printfn "completed job #%d" jobId
}

let job1 = createJob 1 3000

run job1
let job2 = createJob 2 5000

Job.conIgnore [job1; job2] |> run


type Product = { Id : int; Name : string}
type Review = {ProductId : int;Author : string; Comment : string} 

let getProduct id = job {
  do! timeOutMillis 2000
  return {Id = id; Name = "My Awesome Product"}
}

let getProductReviews id = job {
  do! timeOutMillis 3000
  return [
    {ProductId = id; Author = "John"; Comment = "It's awesome!"}
    {ProductId = id; Author = "Sam"; Comment = "Great product"}
  ]
}

type ProductWithReviews = { 
  Id : int
  Name : string
  Reviews : (string * string) list
}

let getProductWithReviews id = job {
  let! product = getProduct id
  let! reviews = getProductReviews id
  return {
    Id = id
    Name = product.Name
    Reviews = reviews |> List.map (fun r -> r.Author,r.Comment)
  }
}

#time
getProductWithReviews 1 |> run
#time

open Hopac.Infixes

let getProductWithReviews2 id = job {
  let! product, reviews = getProduct id <*> getProductReviews id
  return {
    Id = id
    Name = product.Name
    Reviews = reviews |> List.map (fun r -> r.Author,r.Comment)
  }
}                            

#time
getProductWithReviews2 1 |> run
#time                          