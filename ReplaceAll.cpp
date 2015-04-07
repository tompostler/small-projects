/* Tom Postler, 2013-12-05
 * A replace all C++ function.
 */

#include <iostream>

std::string replaceAll(std::string target, std::string old_val, std::string new_val)
{
    std::string result;
    int pos = 0;
    int match_pos = 0;
    while ((match_pos = target.find(old_val, pos)) != std::string::npos)
    {
        result += target.substr(pos, match_pos - pos);
        result += new_val;
        pos = match_pos + old_val.size();
    }
    result += target.substr(pos, std::string::npos);
    return result;
}

int main()
{
    std::string foo, bar;
    foo = "There! Over There!!";
    bar = ReplaceAll(foo, "There", "Here");
    std::cout << foo << "\n" << bar << "\n";
    return 0;
}
