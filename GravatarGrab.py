# Tom Postler, 2015-03-31
# Given an email, get the corresponding image from Gravatar

import urllib.request, urllib
import hashlib
import sys

# Check
if len(sys.argv) != 3:
    print("Usage: python3", sys.argv[0], "emailAddress \"file name\"")
    sys.exit()

# Email
print("Email:", sys.argv[1])
email = sys.argv[1]

# Default image and size to retreive
default = "404"
size = "80"
form = ".png"

# Assemble the URL
gurl = "http://gravatar.com/avatar/"
gurl += hashlib.md5(email.lower().encode()).hexdigest() + form
gurl += "?" + urllib.parse.urlencode({'d': default, 's':size})

# Retrieve the image
try:
    img = urllib.request.urlopen(gurl)
except urllib.error.HTTPError as e:
    print("Error with request: {} {}".format(e.code, e.reason))
    sys.exit()

# Write the image
fname = sys.argv[2] + form
print("Fname:", fname)
file = open(fname, 'wb')
file.write(img.read())
file.close()
