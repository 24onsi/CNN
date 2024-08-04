#ifndef HANDLER_H
#define HANDLER_H

#include <iostream>
#include <cstdlib>
#include <cstring>    
#include <mariadb/conncpp.hpp> 
#include <nlohmann/json.hpp> 
#include <unistd.h>
#include <arpa/inet.h>
#include "DATA.h"

using json = nlohmann::json;


class DB
{
public:
    DB();
    ~DB();
    sql::Connection* Connect();
    void Disconnect(sql::Connection* conn);

private:

};


class Client
{
public:
    Client();
    ~Client();

    void ID_Check(const Data & data, int sock);
    void Login(const Data & data, int sock);
    void Join(const Data & data, int sock);
    void RangeNum(sql::Connection*con, std::string table, std::string column);
    void InsertDB_Hitsory(const Data & data);
    void Artist_Info(int sock);
    void Search_Artist(const Data & data, int sock);


private:
    DB db;
};

#endif