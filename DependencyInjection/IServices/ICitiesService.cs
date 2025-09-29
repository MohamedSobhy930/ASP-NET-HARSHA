using Models;

namespace IServices
{
    public interface ICitiesService
    {
        public List<CityWeather> GetCityWeathers();
        public CityWeather GetCityWeather(string cityUniqueCode);
    }
}
