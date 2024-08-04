#include "HANDLER.cpp"

using json = nlohmann::json;

#define PORT_NUM 5001

void error_handling(const char *msg);
void *handle_clnt(void *arg);

int main()
{
    int serv_sock;
    struct sockaddr_in serv_adr;

    serv_sock = socket(PF_INET, SOCK_STREAM, 0);

    if (serv_sock == -1)
    {
        error_handling("서버 소켓 생성 실패");
    }

    memset(&serv_adr, 0, sizeof(serv_adr));

    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = INADDR_ANY;
    serv_adr.sin_port = htons(PORT_NUM);


    if (bind(serv_sock, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0)
    {
        error_handling("서버 소켓 바인딩 실패");
    }

    if (listen(serv_sock, 5) == -1)
    {
        error_handling("서버 소켓 리슨 실패");
    }

    while (1)
    {
        int clnt_sock;
        struct sockaddr_in clnt_adr;
        socklen_t clnt_adr_sz = sizeof(clnt_adr_sz);

        clnt_sock = accept(serv_sock, (struct sockaddr *)&clnt_adr, &clnt_adr_sz);

        if (clnt_sock == -1)
        {
            std::cerr << "클라이언트 연결 수락 실패";
            close(clnt_sock);
        }

        pthread_t clnt_thread;
        pthread_create(&clnt_thread, nullptr, handle_clnt, (void *)&clnt_sock);
        pthread_detach(clnt_thread);
    }


    close(serv_sock);

    return 0;
}


void *handle_clnt(void *arg)
{
    int clnt_sock = *((int*)arg);
    char buffer[1024] = {0};

    try
    {
        Data ReadData;
        Client clnt;

        int read = recv(clnt_sock, buffer, 1024, 0);

        std::string temp(buffer, read);
        json ReadJson = json::parse(temp);
        json ListJson;

        std::cout << ReadJson << std::endl;

        ReadData.Key = ReadJson.value("Key", 0);

        std::cout << "sock: " << clnt_sock << "Key: " << ReadData.Key << std::endl;

        switch (ReadData.Key)
        {
            case ID_CHECK:
                std::cout << "ID_CHECK" << std::endl;
                ReadData.user.ID = ReadJson.value("ID", "");

                clnt.ID_Check(ReadData, clnt_sock);

                break;

            case LOGIN:
                std::cout << "LOGIN" << std::endl;
                ReadData.user.ID = ReadJson.value("ID", "");
                ListJson = ReadJson.value("user", json::array());
                ReadData.user.PW = ListJson[0].value("PW", "");

                clnt.Login(ReadData, clnt_sock);

                break;

            case JOIN:
                std::cout << "JOIN" << std::endl;
                ReadData.user.ID = ReadJson.value("ID", "");
                ListJson = ReadJson.value("user", json::array());
                ReadData.user.PW = ListJson[0].value("PW", "");
                ReadData.user.Name = ListJson[0].value("Name", "");
                ReadData.user.PhoneNum = ListJson[0].value("PhoneNum", "");

                clnt.Join(ReadData, clnt_sock);

                break;

            case ARTIST:
                std::cout << "ARTIST" << std::endl;
                clnt.Artist_Info(clnt_sock);

                break;

            case RESULT:
                std::cout << "RESULT" << std::endl;
                ReadData.user.ID = ReadJson.value("ID", "");
                ReadData.artist.Artist = ReadJson.value("Value", "");

                clnt.Search_Artist(ReadData, clnt_sock);
                break;

            default:
                break;
        }
    }
    
    catch(const std::exception& e)
    {
        std::cerr << e.what() << '\n';
        try
        {
            json js = json{{"Type", FAIL}};
            std::string sendData = js.dump();
            write(clnt_sock, sendData.c_str(), sendData.length());
        }
        catch(const std::exception& e)
        {
            std::cerr << e.what() << std::endl;
        }
    }

    std::cout << clnt_sock << " : 접속종료" << std::endl;
    close(clnt_sock);
    return nullptr;
}

void error_handling(const char *message)
{
    fputs(message, stderr);
    fputc('\n', stderr);
    exit(1);
}

