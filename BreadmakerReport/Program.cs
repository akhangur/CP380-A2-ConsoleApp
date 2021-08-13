using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using RatingAdjustment.Services;
using BreadmakerReport.Models;

namespace BreadmakerReport
{
    class Program
    {
        static string dbfile = @".\data\breadmakers.db";
        static RatingAdjustmentService ratingAdjustmentService = new RatingAdjustmentService();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Bread World");
            var BreadmakerDb = new BreadMakerSqliteContext(dbfile);
            var t= BreadmakerDb.Breadmakers
                .Select(line => new
                {
                    detail = line.title,
                    reviews = line.Reviews.Count(),
                    avg = (Double)BreadmakerDb.Reviews
                        .Where(i =>i.BreadmakerId == line.BreadmakerId)
                        .Select(i => i.stars).Sum() / line.Reviews.Count(),
                })
                .ToList();

            var BMList = t
                .Select(line => new
                {
                    detail = line.detail,
                    reviews = line.reviews,
                    avg = line.avg,
                    adjust = ratingAdjustmentService.Adjust(line.avg, line.reviews)
                })
                .OrderByDescending(i => i.adjust)
                .ToList();


            Console.WriteLine("[#]  Reviews  Average  Adjust  Description");
            for (var j = 0; j < 3; j++)
            {
                var line = BMList[j];
                Console.WriteLine($"[{j + 1}]  {line.reviews,7}  {Math.Round(line.avg, 2),-7}  {Math.Round(line.adjust, 2),-6}   {line.detail}");
            }
        }
    }
}
