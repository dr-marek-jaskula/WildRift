process.env.NODE_TLS_REJECT_UNAUTHORIZED="0";
import Image from "next/image";
import Link from "next/link";
import styles from "../../styles/ChampionPage.module.scss";
import { useState, useEffect } from "react";
import Wrapper from "../../components/Wrapper";
import { Checkbox, FormControlLabel, FormGroup } from "@mui/material";

const ChampionsPage = (props) => {

	const { darkmode, toggleDarkMode, championNames } = props;
	const processToFilename = (string) => {
		return string.replace(/[^a-zA-Z0-9]/g, "");
	};
	
	const fixedChampionNames = championNames.map((item) => ({...item, params: {imgName: processToFilename(item.params.name), name: item.params.name}}));
	const [ championList ] = useState(fixedChampionNames);
	const [ championTypes ] = useState(["Assassin", "Defender", "Mage", "Marksman", "Support", "Warrior"]);
	const [ filter, setFilter ] = useState("");
	const initialChampionTypesObject = {};
	
	useEffect(() => {
		championTypes.forEach(type => initialChampionTypesObject[type] = false);
	}, []);

	const [selectedTypes, setSelectedTypes] = useState(initialChampionTypesObject);

	const handleCheck = (e) => {
		const item = e.target.value;
		setSelectedTypes({
			...selectedTypes,
			[item]: !selectedTypes[item]
		});
	};

	const handleSearch = (e) => {
		setFilter(e.target.value);
	};

	return (
		<Wrapper darkmode={darkmode}>
			<main className={styles.championsPage}>
				<h1 className={styles.mainHeader}>Champions</h1>
				<div className={styles.filters}>
					<div className={styles.searchBarWrapper}>
						<label htmlFor="championSearch" className={styles.championFilterLabel}>Search by name</label>
						<input onChange={handleSearch} placeholder="Start typing the champion name here" id="championSearch" name="championSearch" type="text"/>
					</div>
					<div className={styles.championTypesFilters}>
						{championTypes.map(item => (
							<div key={item} className={styles.championTypeCheckbox}>
								<FormGroup classes={{root: styles.checkboxWrapper}}>
									<FormControlLabel
										onChange={handleCheck} 
										classes={{root: styles.championFilterLabel__root, label: !selectedTypes[item] ? styles.championFilterLabel : styles.championFilterLabelChecked}} 
										labelPlacement="start"
										control={
											<Checkbox 
												classes={{root: styles.checkbox, checked: styles.checked}}
												icon={
													<svg width="19" height="19" viewBox="0 0 19 19" fill="none" xmlns="http://www.w3.org/2000/svg">
														<path d="M16.7917 2.20833V16.7917H2.20833V2.20833H16.7917ZM16.7917 0.125H2.20833C1.0625 0.125 0.125 1.0625 0.125 2.20833V16.7917C0.125 17.9375 1.0625 18.875 2.20833 18.875H16.7917C17.9375 18.875 18.875 17.9375 18.875 16.7917V2.20833C18.875 1.0625 17.9375 0.125 16.7917 0.125Z" fill="#535860"/>
													</svg>
												} 
												checkedIcon={<svg width="19" height="19" viewBox="0 0 19 19" fill="none" xmlns="http://www.w3.org/2000/svg">
													<path d="M16.7917 0.125H2.20833C1.0625 0.125 0.125 1.0625 0.125 2.20833V16.7917C0.125 17.9375 1.0625 18.875 2.20833 18.875H16.7917C17.9375 18.875 18.875 17.9375 18.875 16.7917V2.20833C18.875 1.0625 17.9375 0.125 16.7917 0.125ZM16.7917 16.7917H2.20833V2.20833H16.7917V16.7917ZM15.7396 6.375L14.2708 4.89583L7.40625 11.7604L4.71875 9.08333L3.23958 10.5521L7.40625 14.7083L15.7396 6.375Z" fill="white"/>
												</svg>
												} 
												value={item} />} 
										label={item}/>
								</FormGroup>
							</div>
						))}
					</div>
				</div>
				<div className={styles.championList}>
					{championList.filter(item => {
						if (!filter.length) return true;
						return item.params.name.toUpperCase().includes(filter.toUpperCase());
					}).map((item, index) => {
						const { params } = item;
						const { name, imgName } = params;
						return (
							<Link href={`champions/${name}`} key={index}>
								<div className={styles.championItem} >
									<Image className={styles.championImage} width="120" height="120"  src={`/champions/thumbnails/${imgName}.png`}/>
									<h3 className={styles.championName}>{name}</h3>
								</div>
							</Link>)
						;}) }	
				</div>
			</main>
		</Wrapper>
	);
};


export default ChampionsPage;

export async function getStaticProps() {
	const req = await fetch("https://localhost:5001/api/Champion/Names", {
		method: "GET",
		mode: "cors",
		cache: "default",
		headers: {
			"Content-Type":"application/json",
		}});
	const data = await req.json();
	const championNames = data.map(name => {
		return { params: {
			name
		}};
	});
	return {
		props: {
			championNames
		}
	};
}