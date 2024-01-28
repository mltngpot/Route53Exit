using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;

public class Program
{
    private static void Main(string[] args)
    {
        var client = new AmazonRoute53Client(RegionEndpoint.USEast1);
        try
        {
            var zones = getHostedZones(client);
            foreach (var zone in zones)
            {
                sortRecords(zone, client);
            }

        }
        catch (AmazonRoute53Exception ex)
        {
            if (ex.ErrorCode != null && (ex.ErrorCode.Equals("InvalidAccessKeyId") ||
            ex.ErrorCode.Equals("InvalidSecurity")))
            {
                Console.WriteLine("Please check the provided AWS Credentials.");
                Console.WriteLine("If you haven't signed up for Amazon Route53, please visit http://aws.amazon.com/s3");
            }
            else
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Request ID: " + ex.RequestId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Caught Exception: " + ex.Message);
        }
    }

    private static void sortRecords(HostedZone zone, AmazonRoute53Client client)
    {
        var request = new ListResourceRecordSetsRequest();
        request.HostedZoneId = zone.Id;
        var requestTask = client.ListResourceRecordSetsAsync(request);
        requestTask.Wait();
        var response = requestTask.Result;
        Console.WriteLine($"Host: {zone.Name}");
        Console.WriteLine();
        foreach (var currentRecord in response.ResourceRecordSets)
        {
            if (currentRecord.Type != RRType.MX) continue;
            Console.WriteLine($"Hostname: {currentRecord.Name}");
            Console.WriteLine($"Type: {currentRecord.Type}");
            Console.WriteLine($"Alias: {currentRecord.AliasTarget}");
            Console.WriteLine("Resource Records:");
            foreach(var resourceRecord in currentRecord.ResourceRecords)
            {
                Console.WriteLine($"{resourceRecord.Value}");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.WriteLine();
    }

    private static IEnumerable<HostedZone> getHostedZones(AmazonRoute53Client client)
    {
        var request = new ListHostedZonesRequest();
        var requestTask = client.ListHostedZonesAsync(request);
        requestTask.Wait();
        var response = requestTask.Result;
        return response.HostedZones;
    }
}