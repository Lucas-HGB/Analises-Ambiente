import win32evtlog
import win32api
import win32con
import win32security # To translate NT Sids to account names.
import win32evtlogutil
from collections import Counter

def ReadLog(logType="Application", dumpEachRecord = True):
    # read the entire log back.
    h=win32evtlog.OpenEventLog("localhost", logType)
    numRecords = win32evtlog.GetNumberOfEventLogRecords(h)
    print "There are %d records" % numRecords
    num=0
    exit = False
    events = {}
    while not exit:
        objects = win32evtlog.ReadEventLog(h, win32evtlog.EVENTLOG_BACKWARDS_READ|win32evtlog.EVENTLOG_SEQUENTIAL_READ, 0)
        objects = [f.SourceName for f in objects if f.SourceName != "Software Protection Platform Service" and "Microsoft" not in f.SourceName]
        num += 1
        for obj in objects:
            events[obj] = 0
        for object in objects:
            events[object] = events[object] + 1
            if num == 20:
                exit = True
                break
        #for object in objects:
        #    if dumpEachRecord:
        #        print "Event record from %r generated at %s" % (object.SourceName, object.TimeGenerated.Format())
    print Counter(events)
    win32evtlog.CloseEventLog(h)

def usage():
    print "Writes an event to the event log."
    print "-w : Dont write any test records."
    print "-r : Dont read the event log"
    print "-c : computerName : Process the log on the specified computer"
    print "-v : Verbose"
    print "-t : LogType - Use the specified log - default = 'Application'"


def test():
    # check if running on Windows NT, if not, display notice and terminate
    if win32api.GetVersion() & 0x80000000:
        print "This sample only runs on NT"
        return

    import sys, getopt
    opts, args = getopt.getopt(sys.argv[1:], "rwh?c:t:v")
    computer = None
    do_read = do_write = 1

    logType = "Application"
    verbose = 0

if __name__== '__main__':
    test()
    ReadLog()