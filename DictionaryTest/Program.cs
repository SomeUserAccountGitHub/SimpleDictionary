using SimpleDictionary;


var simpleDict = new SimpleDictionary<string, string>();

simpleDict["2"] = "zz";
Console.WriteLine(simpleDict.ToString());

simpleDict.Add("3", "ab");
Console.WriteLine(simpleDict.ToString());

simpleDict["3"] = "ab1";
Console.WriteLine(simpleDict.ToString());


simpleDict.Add("4", "cd");
Console.WriteLine(simpleDict.ToString());


simpleDict.Clear();
Console.WriteLine(simpleDict.ToString());


simpleDict.Add("5", "ef");
Console.WriteLine(simpleDict.ToString());

simpleDict.Add("6", "gh");
Console.WriteLine(simpleDict.ToString());

simpleDict.Add("7", "ij");
Console.WriteLine(simpleDict.ToString());

simpleDict.Add("8", "kl");
Console.WriteLine(simpleDict.ToString());


simpleDict.Remove("6");
Console.WriteLine(simpleDict.ToString());
simpleDict.Remove("7");
Console.WriteLine(simpleDict.ToString());
simpleDict.Remove("5");
Console.WriteLine(simpleDict.ToString());
var removed = simpleDict.Remove("8");
Console.WriteLine("removed: " + removed.ToString());
Console.WriteLine(simpleDict.ToString());

removed = simpleDict.Remove("8");
Console.WriteLine("removed: " + removed.ToString());
Console.WriteLine(simpleDict.ToString());
