<template>
  <div class="hello">
    <AppDropbox @input="onFileInput" />
    <AppButton @buttonClick="onSendImageClick"> Create Character </AppButton>
  </div>
  <div v-if="this.mrzData" class="mrz-data">
    <span> {{ this.mrzData.type }} </span>
    <span> {{ this.mrzData.countryCode }} </span>
    <span> {{ this.mrzData.name }} </span>
    <span> {{ this.mrzData.surname }} </span>
    <span> {{ this.mrzData.patronymic }} </span>
    <span> {{ this.mrzData.passportNumber }} </span>
    <span> {{ this.mrzData.placeOfBirth }} </span>
    <span> {{ this.mrzData.dateOfBirth }} </span>
    <span> {{ this.mrzData.sex }} </span>
    <span> {{ this.mrzData.identificationNumber }} </span>
    <span> {{ this.mrzData.dateOfExpire }} </span>
  </div>
</template>

<script>
import axios from "axios";

import { AppButton } from "@/components/AppButton";
import { AppDropbox } from "@/components/AppDropbox";

export default {
  name: "HelloWorld",
  components: {
    AppButton,
    AppDropbox,
  },
  data() {
    return {
      uploadFile: null,
      mrzData: null,
    };
  },
  methods: {
    onFileInput({ data }) {
      this.uploadFile = data;
    },
    async onSendImageClick() {
      const formData = new FormData();
      formData.append("passportImage", this.uploadFile);

      try {
        const response = await axios({
          method: "post",
          url: `https://localhost:5001/api/passport`,
          data: formData,
          headers: { "Content-Type": "multipart/form-data" },
        });

        this.mrzData = response.data;
      } catch (e) {
        console.error("Failed to parse image");
      }
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
.mrz-data {
  display: flex;
  flex-direction: column;
}
</style>
