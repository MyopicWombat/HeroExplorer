using HeroExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace HeroExplorer
{
    public class MarvelFacade
    {
        private const string PrivateKey = "b5103c0bb8c98c0a9bf8801837f6851d55dd69b8";
        private const string PublicKey = "ee77766cdbf6c43b2c27cb34d367535f";
        private const int MaxCharacters = 1500;
        private const string ImageNotAvailablePath = "http://i.annihil.us/u/prod/marvel/i/mg/b/40/image_not_available";
        

        public static object Ibuffer { get; private set; }

        public static async Task<string> PopulateMarvelCharactersAsync(ObservableCollection<Character> marvelCharcters)
        {
            try
            {
                var characterDataWrapper = await GetCharacterDataWrapperAsync();

                var characters = characterDataWrapper.data.results;

                foreach (var character in characters)
                {
                    if (character.thumbnail != null && character.thumbnail.path != "" && character.thumbnail.path != ImageNotAvailablePath)
                    {
                        character.thumbnail.small = String.Format("{0}/standard_small.{1}", character.thumbnail.path, character.thumbnail.extension);
                        character.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}", character.thumbnail.path, character.thumbnail.extension);

                        marvelCharcters.Add(character);

                    }


                }
                return characterDataWrapper.attributionText;
            }
            catch (Exception)
            {
                return "";
            }
            

        }

        public async static Task<CharacterDataWrapper> GetCharacterDataWrapperAsync()
        {

            // Assemble the URL
            Random random = new Random();
            var offset = random.Next(MaxCharacters);

            string url = string.Format("https://gateway.marvel.com:443/v1/public/characters?limit=10&offset={0}",offset);

            var jsonMessage = await CallMarvelAsync(url);

            // Response --> String / JSON  --> Deserialize
            //DataContractJsonSerializer
            var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (CharacterDataWrapper)serializer.ReadObject(ms);
            return result;
        }

        public static async Task PopulateMarvelComicsAsync(int characterID, ObservableCollection<Comic> marvelComics)
        {
            try
            {
                var comicDataWrapper = await GetComicDataWrapperAsync(characterID);

                var comics = comicDataWrapper.data.results;

                foreach (var comic in comics)
                {
                    if (comic.thumbnail != null && comic.thumbnail.path != "" && comic.thumbnail.path != ImageNotAvailablePath)
                    {
                        comic.thumbnail.small = String.Format("{0}/portrait_medium.{1}", comic.thumbnail.path, comic.thumbnail.extension);
                        comic.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}", comic.thumbnail.path, comic.thumbnail.extension);

                        marvelComics.Add(comic);
                    }


                }
            }
            catch (Exception)
            {
                return;
            }


        }
        public async static Task<ComicDataWrapper> GetComicDataWrapperAsync(int characterID)
        {
            // Assemble the URL
            
            string url = string.Format("https://gateway.marvel.com:443/v1/public/comics?characters={0}&limit=10&", characterID);

            string jsonMessage = await CallMarvelAsync(url);

            // Response --> String / JSON  --> Deserialize
            //DataContractJsonSerializer
            var serializer = new DataContractJsonSerializer(typeof(ComicDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (ComicDataWrapper)serializer.ReadObject(ms);
            return result;

        }
        private async static Task<string> CallMarvelAsync(string url)
        {
            // Get the MD5 Hash
            var timeStamp = DateTime.Now.Ticks.ToString();
            var hash = CreateHash(timeStamp);

            string completeUrl = string.Format("{0}&apikey={1}&ts={2}&hash={3}", url, PublicKey, timeStamp, hash);

            // Call Marvel API
            HttpClient http = new HttpClient();
            var response = await http.GetAsync(completeUrl);
            return await response.Content.ReadAsStringAsync();
        }

        private static string CreateHash(string timeStamp)
        {
            
            var toBeHashed = timeStamp + PrivateKey + PublicKey;
            var hashedMessage = ComputeMD5(toBeHashed);



            return hashedMessage;
        }

        private static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
