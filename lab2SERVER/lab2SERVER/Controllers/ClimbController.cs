using lab2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace lab2SERVER.Controllers
{
    public class ClimbController : ApiController
    {
        public IEnumerable<SearchResultLine> GetAllHotel()
        {
            List<SearchResultLine> result;

            using (ClimbersClubEntities db = new ClimbersClubEntities())
            {
                var query = (from climberMountains in db.ClimbersMountains
                             join climber in db.Climbers on climberMountains.Climber_Id equals climber.Id
                             join mountain in db.Mountains on climberMountains.Mountain_Id equals mountain.Id
                             select new SearchResultLine
                             {
                                 ClimberName = climber.Name,
                                 ClimberSurname = climber.Surname,
                                 Mountain = mountain.Name,
                                 Id = climberMountains.Id
                             }).ToList(); // Можно вызвать ToList() здесь

                result = query;
            }

            return result;
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/edit/addclimb")]
        public IHttpActionResult AddClimb(int climberId, int mountainId)
        {
            try
            {
                using (ClimbersClubEntities db = new ClimbersClubEntities())
                {
                    var newClimberMountain = new ClimbersMountains
                    {
                        Climber_Id = climberId,
                        Mountain_Id = mountainId
                    };

                    db.ClimbersMountains.Add(newClimberMountain);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/climber/get")]
        public Dictionary<int, string> GetClimbers()
        {
            Dictionary<int, string> climbers = new Dictionary<int, string>();

            try
            {
                using (ClimbersClubEntities db = new ClimbersClubEntities())
                {
                    var climberQuery = from climb in db.Climbers
                                     select new
                                     {
                                         climb.Id,
                                         climb.Name,
                                         climb.Surname
                                     };

                    foreach (var climber in climberQuery)
                    {
                        climbers.Add(climber.Id, $"{(string)climber.Surname} {(string)climber.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return climbers;
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/mountain/get")]
        public Dictionary<int, string> GetMountains()
        {
            Dictionary<int, string> mountains = new Dictionary<int, string>();

            try
            {
                using (ClimbersClubEntities db = new ClimbersClubEntities())
                {
                    var mountainQuery = from moun in db.Mountains
                                    select new
                                    {
                                        moun.Id,
                                        moun.Name,
                                        moun.High
                                    };

                    foreach (var mountain in mountainQuery)
                    {
                        mountains.Add(mountain.Id, $"{mountain.Name} (высота: {mountain.High} м.)");
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return mountains;
        }
        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteClimb(int id)
        {
            try
            {
                using (ClimbersClubEntities db = new ClimbersClubEntities())
                {
                    var climberMountainDelete = db.ClimbersMountains.FirstOrDefault(cm => cm.Id == id);

                    if (climberMountainDelete != null)
                    {
                        db.ClimbersMountains.Remove(climberMountainDelete);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Запись не найдена." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        //public IEnumerable<SearchResultLine> GetClimbBy(int roomN)
        //{
        //    List<SearchResultLine> result;
        //    using (HotelEntities db = new HotelEntities())
        //    {
        //        var query = (from guestRoom in db.GuestRoom
        //                     join guest in db.Guest on guestRoom.id_guest equals guest.id
        //                     join room in db.Room on guestRoom.id_room equals room.id
        //                     where room.number == roomN // Добавляем условие по идентификатору
        //                     select new SearchResultLine
        //                     {
        //                         GuestName = guest.name,
        //                         GuestSurname = guest.second_name,
        //                         Room = room.number ?? 0, // Если number nullable, используйте значение по умолчанию
        //                         Id = guestRoom.id
        //                     });
        //        result = query.ToList<SearchResultLine>();
        //    }
        //    return result;
        //}
    }
}
