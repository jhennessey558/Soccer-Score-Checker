using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;

public class Solution
{
    static public Dictionary<string, int> Run(string input)
    {
        try
        {
            string url = "https://s3.eu-west-1.amazonaws.com/hackajob-assets1.p.hackajob/challenges/football_session/football.json";

            using (HttpClient client = new HttpClient())
            {
                var response = client.GetStringAsync(url).Result;
                var json = JObject.Parse(response);

                if (json["rounds"] == null)
                {
                    throw new Exception("Rounds data is missing in the JSON response.");
                }

                if (input.ToLower() == "all")
                {
                    return GetAllTeamsScores(json);
                }
                else
                {
                    string teamKey = GetTeamKey(json, input);
                    if (teamKey == null)
                    {
                        throw new Exception($"Team '{input}' not found.");
                    }

                    int totalGoals = GetTeamGoals(json, teamKey);
                    return new Dictionary<string, int> { { input, totalGoals } };
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null; // Indicate an error
        }
    }

    private static string GetTeamKey(JObject json, string input)
    {
        foreach (var round in json["rounds"])
        {
            foreach (var match in round["matches"])
            {
                if (match["team1"] != null && (match["team1"]["name"].ToString().ToLower() == input.ToLower() || match["team1"]["code"].ToString().ToLower() == input.ToLower()))
                {
                    return match["team1"]["key"].ToString();
                }
                if (match["team2"] != null && (match["team2"]["name"].ToString().ToLower() == input.ToLower() || match["team2"]["code"].ToString().ToLower() == input.ToLower()))
                {
                    return match["team2"]["key"].ToString();
                }
            }
        }
        return null;
    }

    private static int GetTeamGoals(JObject json, string teamKey)
    {
        int totalGoals = 0;

        foreach (var round in json["rounds"])
        {
            if (round["matches"] == null)
            {
                continue; // Skip this round if matches data is missing
            }

            foreach (var match in round["matches"])
            {
                if (match["team1"] == null || match["team2"] == null || match["score1"] == null || match["score2"] == null)
                {
                    continue; // Skip this match if any required data is missing
                }

                if (match["team1"]["key"].ToString().ToLower() == teamKey.ToLower())
                {
                    totalGoals += (int)match["score1"];
                }
                else if (match["team2"]["key"].ToString().ToLower() == teamKey.ToLower())
                {
                    totalGoals += (int)match["score2"];
                }
            }
        }

        return totalGoals;
    }

    private static Dictionary<string, int> GetAllTeamsScores(JObject json)
    {
        Dictionary<string, int> teamScores = new Dictionary<string, int>();

        foreach (var round in json["rounds"])
        {
            if (round["matches"] == null)
            {
                continue; // Skip this round if matches data is missing
            }

            foreach (var match in round["matches"])
            {
                if (match["team1"] == null || match["team2"] == null || match["score1"] == null || match["score2"] == null)
                {
                    continue; // Skip this match if any required data is missing
                }

                string team1Key = match["team1"]["key"].ToString();
                string team2Key = match["team2"]["key"].ToString();
                int team1Goals = (int)match["score1"];
                int team2Goals = (int)match["score2"];

                if (!teamScores.ContainsKey(team1Key))
                {
                    teamScores[team1Key] = 0;
                }
                if (!teamScores.ContainsKey(team2Key))
                {
                    teamScores[team2Key] = 0;
                }

                teamScores[team1Key] += team1Goals;
                teamScores[team2Key] += team2Goals;
            }
        }

        return teamScores;
    }
}