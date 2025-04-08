using System.ComponentModel.DataAnnotations;

namespace Francesco.Recipes.World.Models.BackendModels.Recipe
{
    public enum Difficulty
    {
        [Display(Name = "Sehr einfach")]
        VeryEasy = 0,

        [Display(Name = "Einfach")]
        Easy = 1,

        [Display(Name = "Mittel")]
        Medium = 2,

        [Display(Name = "Schwer")]
        Hard = 3,

        [Display(Name = "Experte")]
        Expert = 4,
    }
}
