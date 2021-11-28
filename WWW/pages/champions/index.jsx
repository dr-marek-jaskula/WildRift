process.env.NODE_TLS_REJECT_UNAUTHORIZED="0";
import Image from "next/image";
import Link from "next/link";
import styles from "../../styles/ChampionPage.module.scss";
import { useState, useEffect } from "react";
import Wrapper from "../../components/Wrapper";
import { Checkbox, FormControlLabel, FormGroup } from "@mui/material";

const ChampionsPage = (props) => {

	const { darkmode, toggleDarkMode, championNames } = props;
	const fixName = (string) => {
		return string.replace(/[^a-zA-Z0-9]/g, "");
	};
	
	const fixedChamionNames = championNames.map((item) => ({...item, params: {name: fixName(item.params.name)}}));
	const [ championList, setChampionList ] = useState(fixedChamionNames);
	const [championTypes] = useState(["Assassin", "Defender", "Mage", "Marksman", "Support", "Warrior"]);
	const [filter, setFilter] = useState("");
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
								<FormGroup>
									<FormControlLabel
										onChange={handleCheck} 
										classes={{label: !selectedTypes[item] ? styles.championFilterLabel : styles.championFilterLabelChecked}} 
										labelPlacement="start" 
										control={<Checkbox classes={{root: styles.checkbox, checked: styles.checked}} value={item} />} label={item}/>
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
						const { name } = params;
						return (
							<Link href={`champions/${name}`} key={index}>
								<div className={styles.championItem} >
									<Image className={styles.championImage} width="120" height="120"  src={`/champions/thumbnails/${name}.png`}/>
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