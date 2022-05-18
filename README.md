# SPARQL for Humans !!! Adaption !!!

## Introduction SPARQL for Humans

This software is adaption of solution available at https://github.com/gabrieldelaparra/SPARQLforHumans. 
Original solution acts as a backend - it parses and stores Wikidata data into local Lucene database and provides a service `QueryGraph` (http://localhost:59286/api/QueryGraph) which imitates SPARQL query execution over localy stored data. The service is excelent and fast, however drawback of it - for an unexperianced user it might be hard to locate the initial subject entity for building the query. To solve this problem, the autor of SPARQL for Humans suggested frontend - the adaption of RDFExplorer available at https://github.com/gabrieldelaparra/RDFExplorer. It further uses Wikidata API lookup service https://www.wikidata.org/w/api.php to find entities by partial text.

## Why adaption is needed?

The key intention for adaption is to make SPARQL for Humans usable for partial text auto completion suggestions. Wikidata API lookup is fast however there is no control of what we about to find... For example, the user might wish to find entities of specific type only. Ir order to solve that the idea is to create additional services in SPARQL for Humans and utilize data structures created in Lucene.
 
## Setting up environment

- `dotnet SDK` https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-5.0.402-windows-x64-installer
- `git for windows SDK` https://github.com/git-for-windows/build-extra/releases/latest or in my case WSL distribution for Ubuntu also worked just fine, since this is needed for updated version of gzip to sort the large output files.
  - to install `gzip` in `git for windows SDK`, just run * *pacman -S gzip* * command in `Git SDK-64` console (`git for windows SDK` console)
- `git` command execution tool (e.g., `git clone`)
-

### Setting up solution

Setting up solution is straight forward - download correct branch of the repository and build the code 

```bash
# download repository and switch to AutoCompletion branch
$ git clone https://github.com/apiskunovs/SPARQLforHumansAdaption.git
$ git switch AutoCompletion

> Cloning into 'SPARQLforHumans'...
> [...]

# go into folder and build the code
$ cd SPARQLforHumansAdaption
$ dotnet build .

> [...]
> Build succeeded.
>     0 Warning(s)
>     0 Error(s)
```

### Data

Data preparation is mainly identical to original code, however with slight modification. For simplicity purpose all commands and their execution times while loading `latest-truthy.nt.gz` (50GB) are listed below. Be aware that the real execution time depends on network and hardware at hands. 

```bash
# for simplicity do in "SparqlForHumans.CLI" folder
cd ./SparqlForHumans.CLI/

# download full Wikidata dump. depending on network might take ~6h
curl -o latest-truthy.nt.gz "https://dumps.wikimedia.org/wikidatawiki/entities/latest-truthy.nt.gz"

# filter data. Takes ~7.5h.
# Not all information will be needed for our solution. This will produce filtered content and will
# create another document *.filterAll.gz
dotnet run -- -i latest-truthy.nt.gz -f 

# sort data. with 16 parallel process it takes around 3h. With 8 processes ~5h.
# !!! be aware that GZIP tool is expected here. Originally it was offered to be install along with
# GIT SDK and, but in practice the same tool potentially can be used from any Linux/Unix env (WSL 
# on Windows). Recommended sort command is provided as a result of filtering. creates new file
# *.filterAll-Sorted.gz
gzip -dc latest-truthy.filterAll.gz | LANG=C sort -S 200M --parallel=16 -T /tmp --compress-program=gzip | gzip > latest-truthy.filterAll-Sorted.gz

# build entity and property index. It takes ~35h and ~2h accordingly
# now indexing command supports BaseFolder configurability (-b <path>). by default it is set to
# "%userprofile%\SparqlForHumans". This path will be required for server configuration to look 
# indexes into. REMARK: both indexes are expected to be in the same BaseFolder.
dotnet run -- -i latest-truthy.filterAll-Sorted.gz -e 
dotnet run -- -i latest-truthy.filterAll-Sorted.gz -p

# ... OR with BaseFolder specified if default one is not ok
# dotnet run -- -i latest-truthy.filterAll-Sorted.gz -e -b ".\wikibase"
# dotnet run -- -i latest-truthy.filterAll-Sorted.gz -p -b ".\wikibase"
```

## Publishing

While reading this section, the reader must be aware that the author does not have real experince in deployment nor hosting .NET core application. Thus commands mentioned here can be challeged. And if so, don't hesite to post an issue for the project. 
Anyhow the main reasoning for deployment and hosting from limited author understanding are:
- simple "copy/paste" deployment without copying source and bulding it each time.
- a need to run several instances of the application - e.g., it might be useful to separate scope of Wikidata from DBPedia. Thus parametrization of high importance
- a need to host application on predefined addresses

### Publishing

Can be run from Windows environment
The commands below are going to create a folder `.\SPARQLforHumansAdaption\SparqlForHumans.Server\bin\Debug\netcoreapp3.1\publish`
```
cd ./SPARQLforHumansAdaption/SPARQLforHumans.Server
dotnet publish
```

Not tested, but there is a hope it is enough to move this folder to destination host 

### Configurability

'publish' folder contains several configuration files. 
- appsettings.json
  - Logging: 
    - `Logging.LogLevel.Default` - For production normaly "Warning", but in general may hold following values to represent logging mode: "Trace", "Debug", "Information", "Warning"
    - `Logging.LogLevel.System` - For production usually not specified, but can hold same values as "Default" section
    - `Logging.LogLevel.Microsoft` - For production usually not specified, but can hold same values as "Default" section
  - Lucene:
    - `Lucene.EntityIndexPath` - Directory path where Lucene indexed entities are kept. Default "%userprofile%\\SparqlForHumans\\LuceneEntitiesIndex"
    - `Lucene.PropertyIndexPath` - Directory path where Lucene indexed properties are kept. Default "%userprofile%\\SparqlForHumans\\LucenePropertiesIndex"
    - `Lucene.InMemoryEngineEnabled` - if FAAS original service "QueryGraph" not used, then this functionality may be disabled. Default is "true"

### Hosting

For demonstration purposes we will launch app from published folder.
```
cd .\SPARQLforHumansAdaption\SparqlForHumans.Server\bin\Debug\netcoreapp3.1\publish
dotnet SparqlForHumans.Server.dll --urls "http://localhost:5050"
```
... or if needed, the application can be hosted on serveral IP addresses
```
dotnet SparqlForHumans.Server.dll --urls "http://localhost:5050;http://localhost:5051"
```

Be aware that 'publish' folder contains also executable SparqlForHumans.Server.exe which seams to be working as well. Due limited author's undersanding on solution configurability it was not considered as the first option but for some scenarios it might be perfectly suited.

## NEW services

### FindClass

Retrieves only class instances. It is wrapper for `FindInstances` with predefined value `isType=true`. All the rest parametrization is according to `FindInstances` specification. 

### FindIndividuals

Retrieves only non-class instances. It is wrapper for `FindInstances` with predefined value `isType=false`. All the rest parametrization is according to `FindInstances` specification. 

### FindInstances

Retrieves instances by various criterias - by search words, incoming and outgoing properties, class type and even in some cases ID (mostly for conviniece purpose). At least one valid request property must be set to retrieve results.

> http://localhost:59286/api/FindInstances?words=porzingis&instanceOf=Q5&inProp=P3373&outProp=P106&id=Q14834367&limit=20

<details><summary>Example JSON output</summary>

```JSON
[{
        "reverseProperties": [{
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P3373",
                "label": ""
            }
        ],
        "subClass": [],
        "description": "Latvian basketball player",
        "parentTypes": [
            "Q5"
        ],
        "altLabels": [
            "Kristaps Porzingis"
        ],
        "isType": false,
        "rank": 1.7301780386830868e-9,
        "properties": [{
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P106",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P118",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P1412",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P1532",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P19",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P21",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P27",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P31",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P3373",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P413",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P54",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P641",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P647",
                "label": ""
            }, {
                "value": "",
                "rank": 0,
                "domain": [],
                "altLabels": [],
                "description": "",
                "range": [],
                "id": "P735",
                "label": ""
            }
        ],
        "id": "Q14834367",
        "label": "Kristaps Porziņģis"
    }
]
```

</details>

- `words` - Mandatory. Search words to be found in fields Label or AltLabel of the instance. It case-insensitive
  - porzi - will search entities with any word starting with 'porzi' regardles if it is first or last word
  - porzi%20krista - will search entities were 2 words exists - one starting with 'porzi' and second starting with 'krista'
  - \* - special character to search any word
- `instanceOf` - Optional. The class(es) the instance must belong to. It is case-sensitive
  - Q5 - instance must belong to Q5
  - Q5%20Q1 - instance must belong to Q5 or Q1
  - Q5%20AND%20Q1 - instance must belong to both Q5 and Q1 at the same time
- `inProp` - Optional. Exact incoming property Id(s) which must be referencing to the instance.
  - P3373 - incoming properties of instance must contain P3373
  - P3373%20P40 - incoming properties of instance must contain P3373 or P40
  - P3373%20AND%20P40 - incoming properties of instance must contain P3373 and P40 at the same time
  - P3373%20AND%20!P40 - incoming properties of instance must contain P3373 and not P40 at the same time
- `outProp` - Optional. Exact outgoing property Id(s) which must be referenced by the instance.
  - P106 - outgoing properties of the instance must contain P106
  - P106%20P118 - outgoing properties of the instance must contain P106 or P118
  - P106%20AND%20P118 - outgoing properties of the instance must contain P106 and P118 at the same time
  - P106%20AND%20!P118 - outgoing properties of the instance must contain P106 and not P118 at the same time
- `id` - Optional. For convinience purpose this service is able to retrieve exact instances by ID
  -id=Q123 - exact id number to search
  -id=Q123%20Q321 - one or another exact id number to search
- `idNot` - Optional. Exlude certains IDs from search results. Syntax same as for `id`
  -id=Q123 - exact id number to skip in search
  -id=Q123%20Q321 - exact id numbers to skip in search. None of them can be in results.
- `isType` - Optional. If set to `true` then only class instances gets returned. If set to `false` then non-class instanctes gets returned by the function.
- `limit` - Optional (default is 20). The max number of returned properties

### FindProperties

Retrieves properties by various criterias - by search words, domain and range classes (where `domain` classes are holding this property as outgoing property and `range` classes artypes which useses parituclar property, and range classes are instance types which) and even in some cases by ID (mostly for conviniece purpose). At least one valid request property must be set to retrieve results.

> http://localhost:59286/api/FindProperties?words=&domain=11424&range=5&id=P4082&limit=20

<details><summary>Example JSON output</summary>

```JSON
[{
        "value": "",
        "rank": 18,
        "domain": [
            11424,
            1358344,
            15416,
            125191,
            2736,
            111241092,
            24862,
            1030329,
            21076217,
            838948
        ],
        "altLabels": [
            "camera model",
            "camera used",
            "equipment used",
            "filmed with",
            "image captured with",
            "photographed with",
            "recorded with",
            "recording device",
            "taken with"
        ],
        "description": "equipment (e.g. model of camera, lens microphone), used to capture this image, video or audio file",
        "range": [
            20888711,
            20741022,
            15328,
            20888659,
            1439691,
            172839,
            214609,
            6145694,
            5,
            811701
        ],
        "id": "P4082",
        "label": "captured with"
    }
]
```

</details>

- `words` - Optoinal. Search words to be found in fields Label or AltLabel of the property. It case-insensitive
  - image - will search properties with any word starting with 'image' regardles if it is first or last word
  - image%20photo - will search properties were 2 words exists - one starting with 'image' and second starting with 'photo'
- `domain` - Optional. Numberic representation of class ('Q' removed from te beginning) which is expected at the incoming side of the property.
  - 5 - property's incoming instance class is expected to be Q5
  - 5%2020888711 - property's incoming instance class is expected to be Q5 or Q20888711 
  - 5%20AND%2020888711 - property's incoming instance class is expected to be Q5 and Q20888711 at the same time
- `range` - Optional. Numberic representation of class ('Q' removed from te beginning) which is expected at the outgoing end of the property.
  - 5 - property's outgoing instance class is expected to be Q5
  - 5%2020888711 - property's outgoing instance class is expected to be Q5 or Q20888711 
  - 5%20AND%2020888711 - property's outgoing instance class is expected to be Q5 and Q20888711 at the same time
- `id` - Optional. For convinience purpose this service is able to retrieve exact properties by ID
  -P123 - exact id number to search
  -P123%20P321 - one or another exact id number to search
- `idNot` - Optional. Exlude certains IDs from search results. Syntax same as for `id`
  -id=Q123 - exact id number to skip in search
  -id=Q123%20Q321 - exact id numbers to skip in search. None of them can be in results.
- `limit` - Optional (default is 20). The max number of returned properties


### GetNextProperties

! Experimental
The service which is made of 2 requests. 1st query retrieves 20000 Lucene document samples whith specified **incoming** property criteria. Mid process collects unique **outgoing** properties. And finally 2nd query reads Properties index by id to retreive their importance and sort.
Out of all new services, _ _GetNextProperties_ _ and _ _GetPrevProperties_ _ are considered to be the most time consuming services as during their process they execute 2 query statements over the indexes.
Same data structure is returned as for `FindProperties` service.

> http://localhost:59286/api/GetNextProperties?id=P3373&limit=100

- `id` - Mandatory. Property id for which next possible properties to be returned.
- `limit` - Optional (default is 20). The max number of returned properties

### GetPrevProperties

! Experimental
Similar to previous one, this service is made of 2 requests, but with a little bit oposite logic. 1st query retrieves 20000 Lucene document samples whith specified **outgoing** property criteria. Mid process collects unique **incoming** properties. And finally 2nd query reads Properties index by id to retreive their importance and sort. 
Out of all new services, _ _GetNextProperties_ _ and _ _GetPrevProperties_ _ are considered to be the most time consuming services as during their process they execute 2 query statements over the indexes.
Same data structure is returned as for `FindProperties` service.

> http://localhost:59286/api/GetPrevProperties?id=P19&limit=100

- `id` - Mandatory. Property id for which previous possible properties to be returned.
- `limit` - Optional (default is 20). The max number of returned properties

--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------
Below you can read original README doc for SPARQL for Humans copied at the moment of creation of this repository. For more up-to-date solution and documentation please refer to original link https://github.com/gabrieldelaparra/SPARQLforHumans
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------

# SPARQL for Humans

Paper\
https://aidanhogan.com/docs/sparql-autocompletion.pdf

# Using this repository

You will first need a Wikidata dump.

## Wikidata Dump

- Wikidata dump latest truthy (`.nt.gz` is larger, but faster for building the index.) https://dumps.wikimedia.org/wikidatawiki/entities/latest-truthy.nt.gz [~50GB]

- For testing you can use an internal file: `SPARQLforHumans\Sample500.nt`

Then some tools for compiling the source code.

## Development tools

- `dotnet SDK` https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-5.0.402-windows-x64-installer
- `git for windows SDK` https://github.com/git-for-windows/build-extra/releases/latest
> `Git For Windows SDK` since need an update version of `gzip` to sort the large output files

- For the `RDFExplorer` client, we will also need `node` https://nodejs.org/en/download/
> If your planning on running the benchamarks only, then `node` is not required.

### Set up gzip
On the `Git SDK-64` console (`Git for Windows SDK Console`)\
Install `gzip` via `pacman`

``` bash
$ pacman -S gzip
```

We'll now need to clone the repository and build the project.

## Clone and build

On the `Git SDK-64` console
``` bash
$ git clone https://github.com/gabrieldelaparra/SPARQLforHumans.git

> Cloning into 'SPARQLforHumans'...
> [...]

$ cd SPARQLforHumans

$ dotnet build .

> [...]
> Build succeeded.
>     0 Warning(s)
>     0 Error(s)
```

Now we will run some tests to check that everything works.

## Test
``` bash
$ cd SparqlForHumans.UnitTests/

$ dotnet test

> [...]
> Passed!  - Failed:     0, Passed:   214, Skipped:     0, Total:   214, Duration: 9 s - SparqlForHumans.UnitTests.dll (netcoreapp3.1)
```

If any of the tests do not pass, you can create an issue and I will get in touch with you :)\
Now we will run the Command Line Interface to filter and index our Wikidata dump.

## Command line interface

``` bash
$ cd ../SparqlForHumans.CLI/

$ dotnet run -- --version
> SparqlForHumans.CLI 1.0.0
```

For the following sections a given `Sample500.nt` file is given on the root folder of the repository.\
To build the complete index (production), `latest-truthy.nt.gz` should be used.

> **Note that filtering, sorting and indexing the `latest-truthy.nt.gz` will take `~40 hours`, depending on your system.**

## Filter

Filters an input file:
- Keeps all subjects that start with `http://www.wikidata.org/entity/`
- Keeps all predicates that start with `http://www.wikidata.org/prop/direct/`
  - and object starts with `http://www.wikidata.org/entity/`
- or predicate is `label`, `description` or `alt-label`
  - and object is literal and ends with `@en`.

These can be changed on the `SparqlForHumans.RDF\FilterReorderSort\TriplesFilterReorderSort.IsValidLine()` method.

To filter run:
``` bash
$ dotnet run -- -i ../Sample500.nt -f
```

The command for `sorting` is given in the console after filtering.\
It will add the `.filterAll.gz` sufix as filtered output and `.filterAll-Sorted.gz` for sorting.

> Filter for `latest` takes `~10 hours` on my notebook computer (16GB RAM).

## Sort

Sorting takes `Sample500.filterAll.gz` as input and outputs `Sample500.filterAll-Sorted.gz`.

> The sorting command process gives no notifications about the status.\
> Sorting `latest` takes `~5 hours` and requires `3x` the size of `Filtered.gz` disk space (`~40GB` free for `latest`)

``` bash
$ gzip -dc Sample500.filterAll.gz | LANG=C sort -S 200M --parallel=4 -T tmp/ --compress-program=gzip | gzip > Sample500.filterAll-Sorted.gz
```

## Entities Index

After filtering and sorting, we can now create our index. As a note, both "`-e -p`" can be given together for the sample file to generate both Entities and Properties Index. For a large file, it is better to do them in 2 steps.

Entities Index will be created by default at `%homepath%\SparqlForHumans\LuceneEntitiesIndex\`

``` bash
$ dotnet run -- -i Sample500.filterAll-Sorted.gz -e
```

If `-p` was not used above, then we need to create the Properties Index.

> Building the Entities Index takes `~30 hours` to complete.

## Properties Index

``` bash
$ dotnet run -- -i Sample500.filterAll-Sorted.gz -p
```
Properties Index will be created by default at `%homepath%\SparqlForHumans\LucenePropertiesIndex\`

Now our index is ready.
- We can now run our backend via `SparqlForHumans.Server/` using the `RDFExplorer` client.
- Or recreate the results from the paper via `SparqlForHumans.Benchmark/`.

> Building the Properties Index takes `~2 hours` to complete.

## Run Server
The backend will listen to request from a modified version of `RDFExplorer`. First we will need to get the server running:

``` bash
$ cd ../SparqlForHumans.Server/

$ dotnet run
```

With the server running we can now start the client.

## Run Client: RDFExplorer

We will now need another console for this.

```
$ cd `path-for-the-client`

$ git clone https://github.com/gabrieldelaparra/RDFExplorer.git

$ cd RDFExplorer

$ npm install

$ npm start
```

Now browse to `http://localhost:4200/`

## Compare against the Wikidata Endpoint

With the full index we can compare our results agains the `Wikidata Endpoint`.
- `67` Properties (`{Prop}`) have been selected to run `4` type of queries (For a total of `268`)
  - `?var1 {Prop} ?var2 . ?var1 ?prop ?var3 .`
  - `?var1 {Prop} ?var2 . ?var3 ?prop ?var1 .`
  - `?var1 {Prop} ?var2 . ?var2 ?prop ?var3 .`
  - `?var1 {Prop} ?var2 . ?var3 ?prop ?var2 .`
- `268` queries are run against our `Local Index` and the `Remote Endpoint`.
- We will query for `?prop` on both (Local and Remote) and compare the results.
- Running the benchmarks takes `2~3 hours`, due to the 50 seconds timeout if the query cannot be completed on the Wikidata Endpoint.
- The details of the runs will be stored at `benchmark.json`.
- The time results will be summarized at `results.txt`.
- The time results, for each query, will be exported to a `points.csv`. Each row is a query. The `Id` of the query can be found on the `benchmark.json` file as `HashCode`.
- A qualitative comparison (`precision`, `recall`, `f1`), for each query, will be exported to `metrics.csv`. Each row is a query. This will only consider those queries that returned results on the `Remote Wikidata Endpoint`.

``` bash
$ cd ../SparqlForHumans.Benchmark/

$ dotnet run
```
