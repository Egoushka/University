#include <iostream>
#include <fstream>
#include <windows.h>
#include <set>
#include <map>
#include <iomanip>
#include <json-develop/single_include/nlohmann/json.hpp>

using json = nlohmann::json;

void tasker();
void first();
void second();
void third();
void four();
void fifth();
void sixth();

int main() {

    tasker();

    system("pause");
    return 0;
}

// Функція для вибору та запуску завдання
void tasker() {
    std::cout << " Choise the task:\n 1,2,3,4,5,6\n";
    int choose;     // Номер завдання
    std::cin >> choose;
    switch (choose)
    {
    case 1:
        first();
        break;
    case 2:
        second();
        break;
    case 3:
        third();
        break;
    case 4:
        four();
        break;
    case 5:
        fifth();
        break;
    case 6:
        sixth();
        break;
    default:
        break;
    }
}

void first() {
    std::ifstream file("mails.json");   // Відкриття файлу
    json reader;
    char* buffer;   // Масив елементів
    file.seekg(0, std::ios::end);   //Перенос каретки в кінець файлу
    int size = file.tellg();        // Визначення розміру файлу
    file.seekg(0, std::ios::beg);   //Повернення на початок файлу
    buffer = new char[size];    // Динамічне виділення памяті під елементи файлу
    // Змінна для кожного елементу
    char* elem; 
    elem = new char;
    int p2 = 0;
    int count = 0;
    int pairs = 0;  
    std::string str;
    for (int p1 = 0, p3; p1 < size; ++p1) {
        file.read(elem, 1);
        if (*elem != '\n' && *elem != '\r' && *elem != '\r\n' && *elem != '\n\t' && *elem != '\r\t' && *elem != '\t') {

            buffer[p2] = *elem;
            ++p2;
            if (*elem == '{') {
                ++pairs;
            }
            else if (*elem == '}') {
                --pairs;
                if (pairs == 0) {
                    ++count;
                }
            }

        }
    }
    json* j = new json[count];      // Створення масиву пустих структур
    std::string str1;
    pairs = 0;
    int i = 0;      // Змінна для підрахунку кількості листів
    for (int p1 = 0, p3; p1 <= p2; ++p1) {
        str1 += buffer[p1];
        if (buffer[p1] == '{') {
            ++pairs;
        }
        else if (buffer[p1] == '}') {
            --pairs;
            if (pairs == 0) {
                j[i] = json::parse(str1);
                str1 = "";
                ++i;
            }
        }
    }
    std::cout << " Count of letters: " << i << std::endl;
}

void second() {
    std::ifstream file("mails.json");
    json reader;
    char* buffer;
    file.seekg(0, std::ios::end);
    int size = file.tellg();
    file.seekg(0, std::ios::beg);
    buffer = new char[size];
    char* elem;
    elem = new char;
    int p2 = 0;
    int count = 0;
    int pairs = 0;
    std::string str;
    for (int p1 = 0, p3; p1 < size; ++p1) {
        file.read(elem, 1);
        if (*elem != '\n' && *elem != '\r' && *elem != '\r\n' && *elem != '\n\t' && *elem != '\r\t' && *elem != '\t') {

            buffer[p2] = *elem;
            ++p2;
            if (*elem == '{') {
                ++pairs;
            }
            else if (*elem == '}') {
                --pairs;
                if (pairs == 0) {
                    ++count;
                }
            }

        }
    }
    json* j = new json[count];
    std::string str1;
    std::set<std::string> senders;
    pairs = 0;
    int i = 0;
    for (int p1 = 0, p3; p1 <= p2; ++p1) {
        str1 += buffer[p1];
        if (buffer[p1] == '{') {
            ++pairs;
        }
        else if (buffer[p1] == '}') {
            --pairs;
            if (pairs == 0) {
                j[i] = json::parse(str1);
                str1 = "";
                std::string lin = j[i]["headers"]["From"];
                senders.insert(lin);
                ++i;
            }
        }
    }

    for (std::set<std::string>::iterator position = senders.begin(); position != senders.end(); position++) {
        std::cout << *position << std::endl;
    }
}

void third() {
    std::ifstream file("mails.json");
    json reader;
    char* buffer;
    file.seekg(0, std::ios::end);
    int size = file.tellg();
    file.seekg(0, std::ios::beg);
    buffer = new char[size];
    char* elem;
    elem = new char;
    int p2 = 0;
    int count = 0;
    int pairs = 0;
    buffer[size] = 0;
    std::string str;

    for (int p1 = 0, p3; p1 < size; ++p1) {
        file.read(elem, 1);
        if (*elem != '\n' && *elem != '\r' && *elem != '\r\n' && *elem != '\n\t' && *elem != '\r\t' && *elem != '\t') {

            buffer[p2] = *elem;
            ++p2;
            if (*elem == '{') {
                ++pairs;
            }
            else if (*elem == '}') {
                --pairs;
                if (pairs == 0) {
                    ++count;
                }
            }
        }
    }
    std::string searchingWord = "ebass@enron.com";
    json* j = new json[count];
    std::string str1;
    std::map<std::string, int> daysOfWeek;
    std::string strg;
    std::string strg1;
    pairs = 0;
    bool isEmail = true;
    int i = 0;
    for (int p1 = 0, p3; p1 <= p2; ++p1) {
        str1 += buffer[p1];
        if (buffer[p1] == '{')
            ++pairs;
        else if (buffer[p1] == '}') {
            --pairs;
            if (pairs == 0) {
                j[i] = json::parse(str1);
                if (!j[i]["headers"]["To"].is_null()) {
                    std::string lin = j[i]["headers"]["To"];
                    for (int p15 = 0, length = searchingWord.length(); p15 < length; ++p15) {
                        if (lin[p15] != searchingWord[p15]) {
                            isEmail = false;
                            break;
                        }
                    }
                    if (isEmail) {
                        strg = j[i]["headers"]["Date"];
                        for (int i1 = 0; i1 < 3; ++i1)
                        {
                            strg1 += strg[i1];
                        }
                        ++daysOfWeek[strg1];
                        strg.clear();
                        strg1.clear();
                    }
                }
                str1 = "";
                ++i;
                isEmail = true;
            }
        }
    }

    for (std::map<std::string, int>::iterator position = daysOfWeek.begin(); position != daysOfWeek.end(); position++) {
        std::cout << position->first << '\t' << position->second << std::endl;
    }
}

void four() {
    std::ifstream file("mails.json");
    json reader;
    char* buffer;
    file.seekg(0, std::ios::end);
    int size = file.tellg();
    file.seekg(0, std::ios::beg);
    buffer = new char[size];
    char* elem;
    elem = new char;
    int p2 = 0;
    int count = 0;
    int pairs = 0;
    buffer[size] = 0;
    std::string str;

    for (int p1 = 0, p3; p1 < size; ++p1) {
        file.read(elem, 1);
        if (*elem != '\n' && *elem != '\r' && *elem != '\r\n' && *elem != '\n\t' && *elem != '\r\t' && *elem != '\t') {

            buffer[p2] = *elem;
            ++p2;
            if (*elem == '{') {
                ++pairs;
            }
            else if (*elem == '}') {
                --pairs;
                if (pairs == 0) {
                    ++count;
                }
            }

        }
    }
    std::string searchingWord1 = "Shanna Husser", searchingWord2 = "Eric Bass";
    json* j = new json[count];
    std::string str1;
    std::map<std::string, int> messages;
    pairs = 0;
    bool isFirst = true;
    int i = 0;
    for (int p1 = 0, p3; p1 <= p2; ++p1) {
        str1 += buffer[p1];
        if (buffer[p1] == '{')
            ++pairs;
        else if (buffer[p1] == '}') {
            --pairs;
            if (pairs == 0) {
                j[i] = json::parse(str1);
                if (!j[i]["headers"]["X-To"].is_null()) {
                    std::string lin = j[i]["headers"]["X-To"];
                    if (lin == searchingWord1 || lin == searchingWord2) {
                        ++messages[lin];
                    }
                }
                str1.clear();
                ++i;
            }
        }
    }
    for (std::map<std::string, int>::iterator position = messages.begin(); position != messages.end(); position++) {
        std::cout << position->first << '\t' << position->second << std::endl;
    }
}

void fifth() {
    std::ifstream file("mails.json");
    json reader;
    char* buffer;
    file.seekg(0, std::ios::end);
    int size = file.tellg();
    file.seekg(0, std::ios::beg);
    buffer = new char[size];
    char* elem;
    elem = new char;
    int p2 = 0;
    int count = 0;
    int pairs = 0;
    buffer[size] = 0;
    std::string str;
    for (int p1 = 0, p3; p1 < size; ++p1) {
        file.read(elem, 1);
        if (*elem != '\n' && *elem != '\r' && *elem != '\r\n' && *elem != '\n\t' && *elem != '\r\t' && *elem != '\t') {

            buffer[p2] = *elem;
            ++p2;
            if (*elem == '{') {
                ++pairs;
            }
            else if (*elem == '}') {
                --pairs;
                if (pairs == 0) {
                    ++count;
                }
            }

        }
    }
    std::string searchingWord = "ebass@enron.com";
    json* j = new json[count];
    std::string str1;
    std::map<std::string, int> subjects;
    std::string strg;
    std::string strg1;
    pairs = 0;
    bool isEmail = true;
    int i = 0;
    for (int p1 = 0, p3; p1 <= p2; ++p1) {
        str1 += buffer[p1];
        if (buffer[p1] == '{')
            ++pairs;
        else if (buffer[p1] == '}') {
            --pairs;
            if (pairs == 0) {
                j[i] = json::parse(str1);
                if (!j[i]["headers"]["To"].is_null()) {
                    std::string lin = j[i]["headers"]["To"];
                    strg = j[i]["headers"]["Date"];
                    for (int i1 = 12; i1 < 4; ++i1)
                    {
                        strg1 += strg[i1];
                    }

                    if (strg1 == "2000") {
                        std::string lin1 = j[i]["headers"]["Subject"];
                        ++subjects[lin1];

                    }
                    strg.clear();
                    strg1.clear();
                }
                str1.clear();
                ++i;
                isEmail = true;
            }
        }
    }
    for (std::map<std::string, int>::iterator position = subjects.begin(); position != subjects.end(); position++) {
        std::cout << position->first << '\t' << position->second << std::endl;
    }
}

void sixth() {
    std::ifstream file("mails.json");
    json reader;
    char* buffer;
    file.seekg(0, std::ios::end);
    int size = file.tellg();
    file.seekg(0, std::ios::beg);
    buffer = new char[size];
    char* elem;
    elem = new char;
    int p2 = 0;
    int count = 0;
    int pairs = 0;
    buffer[size] = 0;
    std::string str;
    for (int p1 = 0, p3; p1 < size; ++p1) {
        file.read(elem, 1);
        if (*elem != '\n' && *elem != '\r' && *elem != '\r\n' && *elem != '\n\t' && *elem != '\r\t' && *elem != '\t') {

            buffer[p2] = *elem;
            ++p2;
            if (*elem == '{') {
                ++pairs;
            }
            else if (*elem == '}') {
                --pairs;
                if (pairs == 0) {
                    ++count;
                }
            }

        }
    }
    json* j = new json[count];
    std::string str1;
    std::map<std::string, int> daysOfWeek;
    std::string strg;
    std::string strg1;
    pairs = 0;
    int i = 0;
    for (int p1 = 0, p3; p1 <= p2; ++p1) {
        str1 += buffer[p1];
        if (buffer[p1] == '{')
            ++pairs;
        else if (buffer[p1] == '}') {
            --pairs;
            if (pairs == 0) {
                j[i] = json::parse(str1);
                if (!j[i]["headers"]["To"].is_null()) {

                    strg = j[i]["headers"]["Date"];
                    for (int i1 = 0; i1 < 3; ++i1)
                    {
                        strg1 += strg[i];
                    }
                    ++daysOfWeek[strg1];
                    strg.clear();
                    strg1.clear();

                }
                str1.clear();
                ++i;
            }
        }
    }
    for (std::map<std::string, int>::iterator position = daysOfWeek.begin(); position != daysOfWeek.end(); position++)
    {
        std::cout << position->first << '\t' << position->second << std::endl;
    }
}