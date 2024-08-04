#include "HANDLER.h"

DB::DB() {}
DB::~DB() { }

sql::Connection* DB::Connect()
{
    try
    {
        sql::Driver* driver = sql::mariadb::get_driver_instance();
        sql::SQLString url = "jdbc:mariadb://127.0.0.1:3306/ART";
        sql::Properties properties({{"user", "OPERATOR"}, {"password", "1234"}});
        std::cout << "DB 접속 성공" << std::endl;

        return driver->connect(url, properties);   
     }
    catch(sql::SQLException& e)
    {
        std::cerr << "DB 접속 실패: " << e.what() << std::endl;
        exit(1);
    }
}

void DB::Disconnect(sql::Connection* conn)
{
    if (!conn->isClosed())
    {
        conn->close();
        std::cout << "DB 접속 해제" << std::endl;
    }
}


Client::Client() { }
Client::~Client() { }

void Client::ID_Check(const Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "ID 중복 체크" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*ID_Check = con->prepareStatement("SELECT ID FROM USER WHERE ID = ?");

        ID_Check->setString(1, data.user.ID);

        sql::ResultSet*res = ID_Check->executeQuery();

        if(res->rowsCount())
        {
            std::cout << "NO" << std::endl;
            SendJson = json{{"Key", NO}};
        }
        else
        {
            std::cout << "OK" << std::endl;
            SendJson = json{{"Key", OK}};
        }

        db.Disconnect(con);

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());
        std::cout << SendStr << " " << SendByte << std::endl;

    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 조회 실패 : " << e.what() << std::endl;

        SendJson = json{{"Key", ERR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << SendStr << " " << SendByte << std::endl;

    }
}

void Client::Login(const Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "Login 조회" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*login
        = con->prepareStatement("SELECT ID, PW FROM USER WHERE ID = ? AND PW = ?");

        login->setString(1, data.user.ID);
        login->setString(2, data.user.PW);

        sql::ResultSet*res = login->executeQuery();

        if(res->rowsCount())
        {
            std::cout << "OK" << std::endl;

            SendJson = json{{"Key", OK}};
        }
        else
        {
            std::cout << "NO" << std::endl;

            SendJson = json{{"Key", NO}};
        }

        db.Disconnect(con);

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << SendStr << " " << SendByte << std::endl;


    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 조회 실패 : " << e.what() << std::endl;

        SendJson = json{{"Key", ERR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << SendStr << " " << SendByte << std::endl;


    }
}

void Client::Join(const Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "Join 등록" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*join
        = con->prepareStatement("INSERT INTO USER VALUES(DEFAULT, ?, ?, ?, ?)");

        join->setString(1, data.user.ID);
        join->setString(2, data.user.PW);
        join->setString(3, data.user.Name);
        join->setString(4, data.user.PhoneNum);

        join->executeQuery();

        RangeNum(con, "USER", "U_NUM");

        db.Disconnect(con);

        SendJson = json{{"Key", OK}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;

    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 등록 실패 : " << e.what() << std::endl;

        SendJson = json{{"Key", ERR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
        
    }
}

void Client::RangeNum(sql::Connection*con, std::string table, std::string column)
{
    sql::PreparedStatement*NUM1 = con->prepareStatement("ALTER TABLE " + table + " AUTO_INCREMENT=1");         
    NUM1->executeQuery();
    sql::PreparedStatement*NUM2 = con->prepareStatement("SET @COUNT = 0");
    NUM2->executeQuery();
    sql::PreparedStatement*NUM3 = con->prepareStatement("UPDATE " + table + " SET " + column + " = @COUNT:=@COUNT+1");
    NUM3->executeQuery();
}

void Client::InsertDB_Hitsory(const Data & data)
{
    try
    {
        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*history
        = con->prepareStatement("INSERT INTO HISTORY VALUES(DEFAULT, ?, DEFAULT, ?)");
        
        history->setString(1, data.user.ID);
        history->setString(2, data.artist.Artist);


        history->executeQuery();

        RangeNum(con, "HISTORY", "H_NUM");

        std::cout << "검진 데이터 등록" << std::endl;

        db.Disconnect(con);

    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 등록 실패 : " << e.what() << std::endl;
    }
}

void Client::Artist_Info(int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "화가 정보" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*info = con->prepareStatement("SELECT * FROM ARTIST");

        sql::ResultSet*res = info->executeQuery();

        if(res->rowsCount())
        {
            SendJson = json{{"Key", OK},
                            {"ArtistList", json::array()}};

            while(res->next())
            {
                SendJson["ArtistList"].push_back({{"No", res->getInt(1)},
                                                {"KorArtist", res->getString(2)},
                                                {"Artist", res->getString(3)},
                                                {"Years", res->getString(4)},
                                                {"Genre", res->getString(5)},
                                                {"Nationality", res->getString(6)},
                                                {"Url", res->getString(7)}});
            }
        }
        else
        {
            SendJson = json{{"Key", NO}};
        }

        db.Disconnect(con);

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 조회 실패 : " << e.what() << std::endl;

        SendJson = json{{"Key", ERR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
    }
}

void Client::Search_Artist(const Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "결과 검색" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*search
        = con->prepareStatement("SELECT * FROM ARTIST WHERE ENG_ARTIST = ?");

        search->setString(1, data.artist.Artist);

        sql::ResultSet*res = search->executeQuery();

        if(res->rowsCount())
        {
            SendJson = json{{"Key", OK},
                            {"ArtistList", json::array()}};

            while(res->next())
            {
                SendJson["ArtistList"].push_back({{"No", res->getInt(1)},
                                                {"KorArtist", res->getString(2)},
                                                {"Artist", res->getString(3)},
                                                {"Years", res->getString(4)},
                                                {"Genre", res->getString(5)},
                                                {"Nationality", res->getString(6)},
                                                {"Url", res->getString(7)}});
            }
        }
        else
        {
            SendJson = json{{"Key", NO}};
        }

        db.Disconnect(con);

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;

        InsertDB_Hitsory(data);

    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 조회 실패 : " << e.what() << std::endl;

        SendJson = json{{"Key", ERR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
    }
}




