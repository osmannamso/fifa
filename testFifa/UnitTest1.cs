using System;
using System.Collections.Generic;
using System.Linq;
using fifa;
using fifa.Controllers;
using fifa.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace testFifa
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var mock = new Mock<IRepository>();
            mock.Setup(repo => repo.GetAll()).Returns(GetTestLeagues());
            var controller = new LeagueController(mock.Object);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<League>>(viewResult.Model);
            Assert.Equal(GetTestLeagues().Count, model.Count());
        }
        private List<League> GetTestLeagues()
        {
            var leagues = new List<League>
            {
                new League {Id=1, Name="LaLiga Santander", Logo="http://pes-files.ru/_ld/292/37476017.png", Place=4},
                new League{Id=2, Name="Premier League", Logo="https://i.pinimg.com/originals/eb/83/d1/eb83d147be7b1ccfbe98247d009b3e8d.png", Place=4},
                new League{Id=3, Name="Bundesliga", Logo="https://static-s.aa-cdn.net/img/amazon/30600000115444/be3689aabb07fcb20ac5ce1eb93dbcbf", Place=4},
                new League{Id=4, Name="Ligue 1 Conforama", Logo="https://cdn.bleacherreport.net/images/team_logos/328x328/ligue_1.png", Place=4},
                new League{Id=5, Name="Serie A TIM", Logo="http://1.bp.blogspot.com/-ksOx5XM7UCg/VYcijncyYFI/AAAAAAAAAgY/vHOaAh8Tf50/s1600/seriea-logo.png", Place=4},
                new League{Id=6, Name="Eredivisie", Logo="https://2.bp.blogspot.com/-xwb29CFzYMo/W7ALqBOLKpI/AAAAAAAALHA/f-MdcPkUVjsbL2-nALon1xuYIUjL-xRoQCK4BGAYYCw/s400/Eredivisie%2B256x.png", Place=4},
                new League{Id=7, Name="Liga NOS", Logo="https://4.bp.blogspot.com/-kNFAX62puZc/W6fHmPSDtqI/AAAAAAAAKx0/uK5JDip7Tv4oqDSbvvf_TX-BVotMR8diQCLcBGAs/s1600/134.png", Place=4},
                new League{Id=8, Name="Super Lig", Logo="https://www.pesmaster.com/pes-2019/graphics/leaguelogos/emb_0119.png?w=128", Place=4}
            };
            return leagues;
        }
    }
}