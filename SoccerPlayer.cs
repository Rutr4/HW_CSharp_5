using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_CSharp_5
{
    [Serializable]
    public class SoccerPlayer
    {
        public SoccerPlayer(int number, string name, int age, Position position, string photoPath)
        {
            this.number = number;
            this.name = name;
            this.age = age;
            this.position = position;
            this.photoPath = photoPath;
        }
        public SoccerPlayer()
        {
            this.number = 0;
            this.name = "Имя игрока";
            this.age = 0;
            this.position = Position.Нападающий;
            this.photoPath = "../../img/default.png";
        }

        [DisplayName("Фотография"), Category("Информация")]
        public string photoPath { get; set; }
        [DisplayName("Номер игрока"), Category("Информация")]
        [Description("Номер игрока в футбол - это уникальный идентификатор игрока на поле.")]
        public int number { get; set; }
        [DisplayName("Имя игрока"), Category("Информация")]
        public string name { get; set; }
        [DisplayName("Возраст"), Category("Информация")]
        public int age { get; set; }
        [DisplayName("Позиция (амплуа)"), Category("Информация")]
        [Description("Позиция футболиста - это место, на котором игрок играет на поле. Некоторые из позиций в футболе включают в себя нападающего, полузащитника, защитника и вратаря.")]
        public Position position { get; set; }
    }

    public enum Position
    {
        Вратарь,
        Защитник,
        Полузащитник,
        Нападающий
    }
}
