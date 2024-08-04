#ifndef DATA_H
#define DATA_H
#include <string>

struct User
{
    std::string ID;
    std::string PW;
    std::string Name;
    std::string PhoneNum;
};

struct ArtistInfo
{
    int No;
    std::string Artist;
    std::string KorArtist;
    std::string Years;
    std::string Genre;
    std::string Nationality;
    std::string Url;
};

struct Data
{
    int Key;
    
    User user;
    ArtistInfo artist;
};

enum KEY
{
    FAIL = 0,

    ID_CHECK = 10,
    // 11
    JOIN,
    // 12
    LOGIN,
    // 13
    ARTIST,
    // 14
    RESULT,

    OK = 20,
    // 21
    NO,
    // 22
    ERR,

};

#endif