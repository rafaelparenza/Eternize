using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Xabe.FFmpeg;
namespace Eternize
{

    public interface IVideoMaker
    {
        Task Inicio();
    }

    public class VideoMaker : IVideoMaker
    {
        public string VideoSaidas { get; set; } = @$"C:\Eternize\video_vvv.mp4";
        public int DuraçãoImagem { get; set; } = 10; // 10 segundos por imagem

        public async Task Inicio()
        {

            var imagens = new List<string>
            {
                @"C:\Eternize\1.jpg",
                @"C:\Eternize\2.jpg"
            };
            int i = 0;
            List<string> videos = new();
            foreach (var var in imagens)
            {
                i++;
                await CriarVideo(new() { var },i);
                videos.Add(Replacevideo(i));
                
            }

            await ConcatenarVideos(videos);
        }
        public string Replacevideo(int i)
        {
            return VideoSaidas.Replace("vvv", i.ToString());
        }
        public async Task CriarVideo(List<string> imagens,int i)
        {
            var VideoSaida = VideoSaidas.Replace("vvv", i.ToString());           

            if (imagens == null || imagens.Count == 0)
            {
                Console.WriteLine("Lista de imagens está vazia. Adicionando imagens padrão...");
                imagens = new List<string>
            {
                @"C:\Eternize\1.jpg",
                @"C:\Eternize\2.jpg"
            };
            }

            // Verifica se o diretório de saída existe; caso contrário, cria-o
            string outputDirectory = Path.GetDirectoryName(VideoSaida);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Cria um arquivo temporário com a lista de imagens
            string tempFile = Path.Combine(Path.GetTempPath(), "input.txt");
            List<string> lines = imagens.Select(img => $"file '{img}'\nduration {DuraçãoImagem}").ToList();
            lines.AddRange(imagens.Select(img => $"file '{img}'\nduration {DuraçãoImagem}"));
            await File.WriteAllLinesAsync(tempFile, lines);

            // Configura o caminho dos executáveis do FFmpeg (deve ser o caminho para a pasta "bin" do FFmpeg extraído)
            FFmpeg.SetExecutablesPath(@"C:\Eternize\ffmpeg-master-latest-win64-gpl\ffmpeg-master-latest-win64-gpl\bin");

            // Cria a conversão para gerar o vídeo a partir das imagens
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-f concat -safe 0 -i \"{tempFile}\"")
                .AddParameter("-r 30") // Taxa de quadros (30 fps, pode ser ajustada conforme necessário)
                .AddParameter("-c:v libx264") // Codec H.264 para maior compatibilidade
                .AddParameter("-pix_fmt yuv420p") // Formato de pixel compatível
                .SetOutput(VideoSaida);

            // Inicia a conversão
            await conversion.Start();

            Console.WriteLine($"Vídeo criado com sucesso em: {VideoSaida}");
        }

        public string VideoSaidaFinal { get; set; } = @"C:\Eternize\video_final.mp4"; // Caminho para o vídeo de saída

        // Método para concatenar vídeos
        public async Task ConcatenarVideos(List<string> videos)
        {
            if (videos == null || videos.Count == 0)
            {
                Console.WriteLine("A lista de vídeos está vazia.");
                return;
            }

            // Verifica se o diretório de saída existe, caso contrário cria
            string outputDirectory = Path.GetDirectoryName(VideoSaidaFinal);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Cria um arquivo temporário para armazenar os nomes dos vídeos
            string tempFile = Path.Combine(Path.GetTempPath(), "input.txt");

            // Cria o arquivo input.txt com a lista de vídeos a serem concatenados
            var lines = videos.Select(video => $"file '{video}'").ToList();
            await File.WriteAllLinesAsync(tempFile, lines);

            // Define o caminho completo do executável do FFmpeg
            string ffmpegPath = @"C:\Eternize\ffmpeg-master-latest-win64-gpl\ffmpeg-master-latest-win64-gpl\bin";

            // Executa o comando FFmpeg para concatenar os vídeos
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-f concat -safe 0 -i \"{tempFile}\"") // Usando o arquivo de lista de vídeos
                .AddParameter("-c:v libx264") // Codec de vídeo
                .AddParameter("-pix_fmt yuv420p") // Formato de pixel compatível
                .SetOutput(VideoSaidaFinal);

            // Inicia a conversão
            await conversion.Start();

            // Excluir o arquivo temporário após o processamento
            File.Delete(tempFile);

            Console.WriteLine($"Vídeo concatenado criado com sucesso em: {VideoSaidaFinal}");
        }

    }
}
