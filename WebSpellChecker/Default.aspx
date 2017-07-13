<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebSpellChecker._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div>
            <textarea id="TextArea" cols="20" name="TextArea" rows="6" style="display:inline-block;width:44%" runat="server"></textarea>
            <p id="outputText" contenteditable="true" runat="server" style="width:50%;display:inline-block; float:right;height: 177px;"></p>
        </div>
        <div>
            <asp:Button ID="checkButton" runat="server" onclick="checkButton_Click" Text="Sprawdź" Height="44px" Width="356px"></asp:Button>
        </div>
        <ul id="candidatesList" contenteditable="true" runat="server" style="position:absolute;background-color:khaki;border-style:double;list-style-type: none;padding:6px"></ul>
    </section>

    <script>
        $(document).ready(function () {
            $('.showCandidates').on('click', function (e) {
                e.preventDefault();
                e.stopImmediatePropagation();

                var candidates = $(e.target).data('candidates'),
                    list = "",
                    position = getOffset(e.target);

                for (var candidate in candidates) {
                    list += "<li class=candidateSet data-name='" + candidate + "'>" + candidate + ": " + candidates[candidate] + "</li>"
                }

                $('#FeaturedContent_candidatesList').html(list);
                $('#FeaturedContent_candidatesList').css("top", position.top + 6);
                $('#FeaturedContent_candidatesList').css("left", position.left);

                changeCandidate($(e.target));
            });

            function changeCandidate(elem) {
                $('.candidateSet').on('click', function (e) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    elem.html(" " + $(this).data('name'));
                })
            }
           

            function getOffset(el) {
                el = el.getBoundingClientRect();
                return {
                    left: el.left + window.scrollX,
                    top: el.top + window.scrollY
                }
            }
        });
    </script>
</asp:Content>

